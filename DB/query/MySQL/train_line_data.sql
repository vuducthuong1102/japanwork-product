SELECT a.id, IFNULL(b.`region_id`, 0) AS region_id,IFNULL(b.`prefecture_id`, 0) AS prefecture_id, IFNULL(b.`city_id`, 0) AS city_id,b.`detail`,b.`postal_code`,a.station,a.`furigana` FROM station a
LEFT JOIN address b ON a.`address_id` = b.`id`
WHERE 1=1
