USE [ApplicationController]
GO

/****** Object:  StoredProcedure [dbo].[uspInsertFileInfo]    Script Date: 7/3/2018 7:16:50 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[uspInsertFileInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[uspInsertFileInfo]
GO
 
 
/****** Object:  UserDefinedTableType [dbo].[TT_FileInfoDetails]    Script Date: 6/7/2018 7:15:36 PM ******/
IF EXISTS (SELECT * FROM sys.types WHERE is_user_defined = 1 AND name = 'TT_FileInfoDetails')
DROP TYPE [dbo].[TT_FileInfoDetails]
GO

/****** Object: Type Table [dbo].[TT_FileInfoDetails]    Script Date: 6/7/2018 3:57:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE Type [dbo].[TT_FileInfoDetails] as table(
	[FileInfoDetailsId] [bigint]   NULL, 
	[FullFilePath] [varchar](2000)   NULL,
    [FileName] [varchar](500)   NULL,
    [FileExtension] [varchar](255)   NULL,
    [FileSize] [bigint]  NULL,
    [FileLastModifiedDate][datetime]   NULL,
    [InUse]  [bit]   NULL 
)	 

GO
/****** Object:  StoredProcedure [dbo].[uspInsertFileInfo]    Script Date: 6/7/2018 3:56:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Surendra,singh,kushwaha>
-- Create date: <3-Jul-2018>
-- Description:	<insert AWSSQSMsg >
-- =============================================
create PROCEDURE [dbo].[uspInsertFileInfo] 
	@TT_FileInfoDetails [dbo].[TT_FileInfoDetails]   readonly 
	, @filefolderpath varchar(2000)
AS
BEGIN
 declare   @cnt int=0 ;--,@filefolderpath_ varchar(2000)='S:\WWW\InstructorResources';
 SET NOCOUNT ON;
 select   @cnt=count(1)    from @TT_FileInfoDetails
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT On;
	if  @cnt>0
	truncate table [dbo].[tblFileInfoDetails];

	SET NOCOUNT OFF;

    -- Insert statements for procedure here
	INSERT INTO [dbo].[tblFileInfoDetails] 
           (
		        [FullFilePath],
                [FileName],
                [FileExtension],
                [FileSize],
                [FileLastModifiedDate],
                [InUse]

		   )
    select  [FullFilePath],
                [FileName],
                [FileExtension],
                [FileSize],
                [FileLastModifiedDate],
                [InUse]
           from @TT_FileInfoDetails;

   if @cnt=0 --when we call sP second time without sending record in datatable then we will update InUse column 
   begin
    --TODO: After insert we can perform other operation
	 	SET NOCOUNT Off;
	 update o 
	 set [InUse]=1
	 from  [dbo].[tblFileInfoDetails] o inner join [BFWCatalogTMM].[dbo].[tblInstructorResourcesDCUM] t
	  on o.[FileName] =  t.szObjectName
	where o.[FullFilePath]= @filefolderpath+'\'+replace(t.[szWebUrl],'/','\') ;
	 
	end --if @@rowcount=1
 
END
