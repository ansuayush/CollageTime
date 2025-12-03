

Create table Providers
(
ProviderId int identity(1,1) primary key,
ProviderName nvarchar(50),
Gateway nvarchar(max)
)

insert into Providers(ProviderName,Gateway) values 
('AllTel','text.wireless.alltel.com'),
('AT&T','txt.att.net'),
('Boost Mobile','myboostmobile.com'),
('Cricket','sms.mycricket.com'),
('Sprint','messaging.sprintpcs.com'),
('T-Mobile','tmomail.net'),
('US Cellular','email.uscc.net'),
('Verizon','vtext.com'),
('Virgin Mobile','vmobl.com')

ALTER TABLE PersonPhoneNumbers
add ProviderId int

ALTER TABLE PersonPhoneNumbers
ADD FOREIGN KEY (ProviderId)
REFERENCES Providers(ProviderId)