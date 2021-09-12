SELECT acc.AccountId
     , acc.Name
     , REPLACE((STUFF((SELECT CAST(', ' + STR(acci.AccountIncludeId) AS VARCHAR) 
         FROM dbo.AccountInclude acci
         WHERE (acci.AccountId = acc.AccountId) 
         FOR XML PATH ('')), 1, 2, '')),' ', '') AS Locations
FROM Account acc


SELECT * FROM dbo.Spend ORDER BY SpendId DESC

SELECT sop.SpendId, sop.AccountPeriodId, acc.Name FROM dbo.SpendOnPeriod sop
JOIN dbo.AccountPeriod accp ON accp.AccountPeriodId = sop.AccountPeriodId
JOIN dbo.Account acc ON acc.AccountId = accp.AccountId
WHERE sop.SpendId = 5090

exec SpEditSpendViewModelList @pSpendId=5090, @pUsername=N'test', @pAccountPeriodId = 8021