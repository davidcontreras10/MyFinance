Select
       max(case when name='AccountId' then convert(int,StringValue) else '' end) as [AccountId],
       max(case when name='Position' then convert(int,StringValue) else '' end) as [Position]
From parseJSON
(
       '[
	   {"AccountId":55,"Position":15},
       {"AccountId":2,"Position":12},
	   ]'
)
where ValueType = 'int'
group by parent_ID



SELECT *
From parseJSON
(
       '[
	   {"AccountId":55,"Position":"15"},
       {"AccountId":2,"Position":"12"},
	   ]'
)

Select
       CAST((max(case when name='AccountId' then convert(Varchar(50),StringValue) else '' end)) AS INT) as [AccountId],
       max(case when name='Position' then convert(Varchar(50),StringValue) else '' end) as [Position]
From parseJSON
(
       '[
	   {"AccountId":"55","Position":"15"},
       {"AccountId":"2","Position":"12"},
	   {"AccountId":"3","Position":"14"},
	   {"AccountId":"5","Position":"16"},
	   {"AccountId":"4","Position":"17"},
	   {"AccountId":"6","Position":"18"},
	   {"AccountId":"7","Position":"19"}
	   ]'
)
where ValueType = 'string' OR ValueType = 'boolean'
group by parent_ID
