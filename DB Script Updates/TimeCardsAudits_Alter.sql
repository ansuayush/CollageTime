

Drop table [dbo].[TimeCardsAudit]

DROP TRIGGER TimeCards_trigger

ALTER TABLE dbo.TimeCardsAudits
ALTER COLUMN UserId VARCHAR(100)