INSERT INTO station (region_id,prefecture_id,city_id,detail,postal_code,station,furigana)
SELECT
   MY_XML.row.query('region_id').value('.', 'int'),
   MY_XML.row.query('prefecture_id').value('.', 'int'),
   MY_XML.row.query('city_id').value('.', 'int'),
   MY_XML.row.query('detail').value('.', 'NVARCHAR(255)'),
   MY_XML.row.query('postal_code').value('.', 'NVARCHAR(255)'),
   MY_XML.row.query('station').value('.', 'NVARCHAR(255)'),
   MY_XML.row.query('furigana').value('.', 'NVARCHAR(255)')
FROM (SELECT CAST(MY_XML AS xml)
      FROM OPENROWSET(BULK 'D:\Working\JobMarket\DB\data\train_line_17.xml', SINGLE_BLOB) AS T(MY_XML)) AS T(MY_XML)
      CROSS APPLY MY_XML.nodes('data/row') AS MY_XML (row);