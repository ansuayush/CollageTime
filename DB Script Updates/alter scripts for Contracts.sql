alter table Contracts
add EarningsCodeId int,
ADJlimits decimal(18,2)

ALTER TABLE Contracts
ADD FOREIGN KEY (EarningsCodeId)
REFERENCES EarningsCodes(EarningsCodeId)



