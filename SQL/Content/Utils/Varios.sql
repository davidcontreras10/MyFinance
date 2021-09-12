SELECT usaa.UserId, ra.ResourceActionId, ra.ResourceActionName, 
ar.ApplicationResourceId, ar.ApplicationResourceName, ral.ResourceAccessLevelId, ral.ResourceAccessLevelName 
FROM dbo.UserAssignedAccess usaa
JOIN dbo.ResourceAction ra ON ra.ResourceActionId = usaa.ResourceActionId
JOIN dbo.ApplicationResource ar ON ar.ApplicationResourceId = usaa.ApplicationResourceId
JOIN dbo.ResourceAccessLevel ral ON ral.ResourceAccessLevelId = usaa.ResourceAccessLevelId

INSERT INTO dbo.UserAssignedAccess 
(UserId,									ResourceActionId, ApplicationResourceId, ResourceAccessLevelId) VALUES
('71722361-99FF-493F-AF02-2BD0ED7CE676',	1,				  1,					 2					  ),
('71722361-99FF-493F-AF02-2BD0ED7CE676',	1,				  1,					 3					  )