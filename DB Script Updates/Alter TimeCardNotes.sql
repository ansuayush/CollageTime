

ALTER TABLE TimeCardNotes
ADD TimeCardId int not null

ALTER TABLE TimeCardNotes
ADD CONSTRAINT FK_TimecardNotes_TimecardId
FOREIGN KEY (TimeCardId) REFERENCES TimeCards(TimeCardId);