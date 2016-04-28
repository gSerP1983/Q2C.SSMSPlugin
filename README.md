# Q2C.SSMSPlugin
Plugin for SSMS, it converts query to command (select to insert for example)

####**Create temp table example:**####

**Before:** ```select * from sys.all_views```

**Do:** Execute Query To Command... -> Query To Create Temp Table...

**After:**
```
IF OBJECT_ID('tempdb..#Table') IS NOT NULL DROP TABLE #Table
CREATE TABLE #Table (
[name] [nvarchar](250) NOT NULL 
   ,[object_id] [int] NOT NULL 
   ,[principal_id] [int] NULL 
   ,[schema_id] [int] NOT NULL 
   ,[parent_object_id] [int] NOT NULL 
   ,[type] [nvarchar](250) NOT NULL 
   ,[type_desc] [nvarchar](250) NULL 
   ,[create_date] [datetime] NOT NULL 
   ,[modify_date] [datetime] NOT NULL 
   ,[is_ms_shipped] [bit] NULL 
   ,[is_published] [bit] NULL 
   ,[is_schema_published] [bit] NULL 
   ,[is_replicated] [bit] NULL 
   ,[has_replication_filter] [bit] NULL 
   ,[has_opaque_metadata] [bit] NULL 
   ,[has_unchecked_assembly_data] [bit] NULL 
   ,[with_check_option] [bit] NULL 
   ,[is_date_correlation_view] [bit] NULL 
   ,[is_tracked_by_cdc] [bit] NULL 
)
GO
```

####**Query To Insert example:**####

**Before:** ```select top 1 object_id, schema_id, type as name from sys.all_views```

**Do:** Execute Query To Command... -> Query To Insert...

**After:**
```
INSERT INTO TABLE_NAME (
	object_id, schema_id, name
)
VALUES (
	-1072372588		-- object_id
	,3		-- schema_id
	,'V '		-- name
)
GO 
```
