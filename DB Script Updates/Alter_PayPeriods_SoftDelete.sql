ALTER TABLE PayPeriods
ADD 
IsDeleted BIT NULL,
DeletedBy VARCHAR(100) NULL,
UserId VARCHAR(100) NULL,
LastModifiedDate DATETIME NULL

UPDATE PayPeriods SET IsDeleted = 0;