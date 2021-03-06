USE [BassWebTest]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateDHCSDates]    Script Date: 7/25/2018 11:25:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/* Create a table type. */  
--CREATE TYPE DHCSTable AS TABLE   
--( 
--   [CDCRNum] [varchar](6) NOT NULL,
--   [Eligibility Date] DateTime, 
--   [Y/N] char(1)
--);  
--GO  
--Drop Procedure [spUpdateDHCSDates]
--DROP Type [dbo].[DHCSTable]
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spUpdateDHCSDates]
	@DHCStb [dbo].[DHCSTable] Readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from.
	SET NOCOUNT ON;

	--Update  T Set DHCSDate = D.[Eligibility Date] FROM Application  T JOIN @DHCStb D on T.EpisodeID = D.EpisodeID
	--Where T.ApplicationTypeID = 2

	 UPDATE [Application] Set DHCSDate = (CASE WHEN ISNULL([Y/N], 'N') = 'N' THEN NULL ELSE T.[Eligibility Date] END ), 
	                          [OutcomeDate]=(CASE WHEN [OutcomeDate] IS NOT NULL THEN [OutcomeDate] ELSE 
	                                           (CASE WHEN ISNULL([Y/N], 'N') = 'N' THEN GetDate()  ELSE T.[Eligibility Date] END ) END),
							 [ApplicationOutcomeID]= (CASE WHEN ISNULL([Y/N], 'N') = 'N' THEN 1 ELSE 0 END)
	   FROM
		 (SELECT e.EpisodeID, d.[Eligibility Date], d.[Y/N] FROM Episode e INNER JOIN
			 (SELECT [CDCRNum], [Eligibility Date], [Y/N] FROM 
				   (SELECT [CDCRNum], [Eligibility Date], [Y/N],ROW_NUMBER() OVER(PARTITION BY CDCRNum ORDER BY [Eligibility Date] DESC) AS RowNum 
					  FROM @DHCStb)ts 
					 WHERE ts.RowNum =1) d ON e.CDCRNum = d.CDCRNum) T 
	WHERE [Application].EpisodeID = T.EpisodeID AND [Application].ApplicationTypeID = 2

END

GO

GRANT EXECUTE ON [dbo].[spImportSomsRecord] to [ACCOUNTS\Svc_CDCRBASSSQLWte]
GO 

