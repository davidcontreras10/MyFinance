INSERT INTO dbo.EntitiesSupported(EntityName,			EntitySearchKey)
VALUES							 ('Bac San Jose',		'%Banco Bac%'),
								 ('Banco Promerica',	'%Banco Promérica S.A.%'),
								 ('DEFAULT',			'DEFAULT');

INSERT INTO dbo.MethodsSupported (MethodId,	EntitiesSupportedId, Colones)
VALUES							 (1,		1,					 1),
								 (2,		1,					 0),
								 (3,		3,					 1),
								 (4,		3,					 0),
								 (1002,		2,					 1),
								 (1003,		2,					 0)