alter table EarningsCodes
alter column TreatyCode bit 


alter table Employees
add EarningsCodeId int ,
Amount decimal(18,2),
TreatyLimit decimal(18,2)

ALTER TABLE Employees
ADD FOREIGN KEY (EarningsCodeId)
REFERENCES EarningsCodes(EarningsCodeId)