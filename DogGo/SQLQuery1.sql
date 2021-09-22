SELECT d.Id , d.Name , Breed, Notes, ImageUrl, OwnerId, o.Name
FROM Dog d 
JOIN Owner o ON OwnerId = o.Id 
WHERE d.Id = 1