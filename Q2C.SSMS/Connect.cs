using System;
using System.Windows.Forms;
using Extensibility;
using EnvDTE;
using Microsoft.VisualStudio.CommandBars;

namespace Q2C.SSMS
{
    /// <summary>
    /// Enter point for plugin
    /// </summary>
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private const string m_NAME_COMMAND1 = "MyCommand1";
        private const string m_NAME_COMMAND2 = "MyCommand2";

        private DTE _mDte;
        private AddIn _mAddIn;

        private CommandBarPopup _mCommandBarPopup;

        public void OnConnection(object application, ext_ConnectMode connectMode,
           object addInInst, ref Array custom)
        {
            try
            {
                _mDte = (DTE)application;
                _mAddIn = (AddIn)addInInst;

                switch (connectMode)
                {
                    case ext_ConnectMode.ext_cm_UISetup:

                        // Create commands in the UI Setup phase. This phase is called only once when the add-in is deployed.
                        CreateCommands();
                        break;

                    case ext_ConnectMode.ext_cm_AfterStartup:

                        InitializeAddIn();
                        break;

                    case ext_ConnectMode.ext_cm_Startup:

                        // Do nothing yet, wait until the IDE is fully initialized (OnStartupComplete will be called)
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnStartupComplete(ref Array custom)
        {
            InitializeAddIn();
        }

        private void CreateCommands()
        {
            var contextUiGuids = new object[] { };

            _mDte.Commands.AddNamedCommand(_mAddIn, m_NAME_COMMAND1, "MyCommand 1", "MyCommand 1", true, 59,
               ref contextUiGuids, (int)vsCommandStatus.vsCommandStatusSupported);

            _mDte.Commands.AddNamedCommand(_mAddIn, m_NAME_COMMAND2, "MyCommand 2", "MyCommand 2", true, 59,
               ref contextUiGuids, (int)vsCommandStatus.vsCommandStatusSupported);
        }

        private void InitializeAddIn()
        {
            // Retrieve commands created in the ext_cm_UISetup phase of the OnConnection method
            var myCommand1 = _mDte.Commands.Item(_mAddIn.ProgID + "." + m_NAME_COMMAND1);
            var myCommand2 = _mDte.Commands.Item(_mAddIn.ProgID + "." + m_NAME_COMMAND2);

            // Retrieve the context menu of code windows
            var commandBars = (CommandBars)_mDte.CommandBars;
            var codeWindowCommandBar = commandBars["SQL Files Editor Context"];

            // Add a popup command bar
            var myCommandBarControl = codeWindowCommandBar.Controls.Add(MsoControlType.msoControlPopup,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            _mCommandBarPopup = (CommandBarPopup)myCommandBarControl;

            // Change its caption
            _mCommandBarPopup.Caption = "My popup";

            // Add controls to the popup command bar
            myCommand1.AddControl(_mCommandBarPopup.CommandBar, _mCommandBarPopup.Controls.Count + 1);

            myCommand2.AddControl(_mCommandBarPopup.CommandBar, _mCommandBarPopup.Controls.Count + 1);
        }

        public void OnBeginShutdown(ref Array custom)
        {
        }

        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
            try
            {
                if (_mCommandBarPopup != null)
                {
                    // may be disposed
                    _mCommandBarPopup.Delete(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void OnAddInsUpdate(ref Array custom)
        {
        }

        public void QueryStatus(string cmdName, vsCommandStatusTextWanted neededText,
           ref vsCommandStatus statusOption, ref object commandText)
        {
            if (cmdName == _mAddIn.ProgID + "." + m_NAME_COMMAND1)
            {
                statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
            else if (cmdName == _mAddIn.ProgID + "." + m_NAME_COMMAND2)
            {
                statusOption = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            }
        }

        public void Exec(string cmdName, vsCommandExecOption executeOption, ref object variantIn,
           ref object variantOut, ref bool handled)
        {
            if (cmdName == _mAddIn.ProgID + "." + m_NAME_COMMAND1)
            {
                MessageBox.Show("MyCommand1 executed");
            }
            else if (cmdName == _mAddIn.ProgID + "." + m_NAME_COMMAND2)
            {
                MessageBox.Show("MyCommand2 executed");
            }
        }
    }
}