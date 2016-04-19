using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using EnvDTE80;
using Extensibility;
using EnvDTE;
using Microsoft.SqlServer.Management.Smo.RegSvrEnum;
using Microsoft.SqlServer.Management.UI.VSIntegration;
using Microsoft.VisualStudio.CommandBars;
using Q2C.Core;
using System.Runtime.InteropServices;


namespace Q2C.SSMS
{
    /// <summary>
    /// Enter point for plugin
    /// </summary>
    [Guid("A2305F50-41F2-4F6E-9EB5-0F594613CB8C")]
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private const string Ads = "https://github.com/gSerP1983";
        private const string SqlFileEditorContextMenuId = "SQL Files Editor Context";
        private const string Query2CommandName = "Execute Query To Command...";

        private DTE _dte;
        private AddIn _addIn;

        /// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
        /// <param name="application">Root object of the host application.</param>
        /// <param name="connectMode">Describes how the Add-in is being loaded.</param>
        /// <param name="addInInst">Object representing this Add-in.</param>
        /// <param name="custom"></param>
        /// <seealso class='IDTExtensibility2' />
        public void OnConnection(object application, ext_ConnectMode connectMode,
           object addInInst, ref Array custom)
        {
            try
            {
                _dte = (DTE)application;
                _addIn = (AddIn)addInInst;

                // For SSMS this need to be set to ext_ConnectMode.ext_cm_Startup rather than ext_ConnectMode.ext_cm_UISetup
                if (connectMode == ext_ConnectMode.ext_cm_Startup)
                {                    
                    var contextMenu = ((CommandBars)_dte.CommandBars)[SqlFileEditorContextMenuId];
                    var query2CommandMenu = (CommandBarPopup) contextMenu.Controls.Add(MsoControlType.msoControlPopup,
                        Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    query2CommandMenu.Caption = Query2CommandName;

                    var contextUiGuids = new object[] { };
                    foreach (var cmd in Factory.GetCommands())
                    {
                        var command =_dte.Commands.Cast<Command>()
                            .FirstOrDefault(x => x.Name == _addIn.ProgID + "." + cmd.Id);
                        if (command == null)
                        {
                            command = _dte.Commands.AddNamedCommand(_addIn, cmd.Id, cmd.Name, cmd.Description, true, 0,
                                ref contextUiGuids, (int) vsCommandStatus.vsCommandStatusSupported);
                        }
                        command.AddControl(query2CommandMenu.CommandBar, query2CommandMenu.Controls.Count + 1);
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param name='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                foreach (var cmd in Factory.GetCommands())
                {
                    var command = _dte.Commands.Cast<Command>()
                        .FirstOrDefault(x => x.Name == _addIn.ProgID + "." + cmd.Id);
                    if (command != null)
                        command.Delete();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param name='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        /// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
        /// <param name='commandName'>The name of the command to determine state for.</param>
        /// <param name='neededText'>Text that is needed for the command.</param>
        /// <param name='status'>The state of the command in the user interface.</param>
        /// <param name='commandText'>Text requested by the neededText parameter.</param>
        /// <seealso class='Exec' />
        public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText,
           ref vsCommandStatus status, ref object commandText)
        {
            if (Factory.GetCommands().Any(cmd => commandName == _addIn.ProgID + "." + cmd.Id))
                status = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }

        /// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
        /// <param name='commandName'>The name of the command to execute.</param>
        /// <param name='executeOption'>Describes how the command should be run.</param>
        /// <param name='variantIn'>Parameters passed from the caller to the command handler.</param>
        /// <param name='variantOut'>Parameters passed from the command handler to the caller.</param>
        /// <param name='handled'>Informs the caller if the command was handled or not.</param>
        /// <seealso class='Exec' />
        public void Exec(string commandName, vsCommandExecOption executeOption, ref object variantIn, ref object variantOut, ref bool handled)
        {
            foreach (var cmd in Factory.GetCommands())
            {
                if (commandName != _addIn.ProgID + "." + cmd.Id) 
                    continue;

                var document = ((DTE2)ServiceCache.ExtensibilityModel).ActiveDocument;
                if (document != null)
                {
                    var selection = (TextSelection) document.Selection;
                    var query = selection.Text;
                    if (string.IsNullOrWhiteSpace(query))
                    {
                        selection.SelectAll();
                        query = selection.Text;
                    }

                    if (!string.IsNullOrWhiteSpace(query))
                    {
                        var connInfo = ServiceCache.ScriptFactory.CurrentlyActiveWndConnectionInfo;
                        var newText = cmd.Execute(GetConnectionString(connInfo.UIConnectionInfo), query);
                        selection.Insert(
                            "/*" + Environment.NewLine + Ads + Environment.NewLine + query + Environment.NewLine + "*/" +
                            Environment.NewLine + newText,
                            (Int32) vsInsertFlags.vsInsertFlagsContainNewText);
                    }
                }

                handled = true;
                return;
            }
        }

        private static string GetConnectionString(UIConnectionInfo connection)
        {
            var integratedSecurity = string.IsNullOrEmpty(connection.Password);
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = connection.ServerName,
                IntegratedSecurity = integratedSecurity,
                InitialCatalog = connection.AdvancedOptions["DATABASE"] ?? "master",
                ApplicationName = "Q2C"
            };
            if (!integratedSecurity)
            {
                builder.Password = connection.Password;
                builder.UserID = connection.UserName;
            }
            return builder.ConnectionString;
        }
    }
}