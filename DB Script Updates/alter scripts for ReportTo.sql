
alter table employees
add ReportToPersonId int

ALTER TABLE Employees
ADD FOREIGN KEY (ReportToPersonId)
REFERENCES persons(personId)



