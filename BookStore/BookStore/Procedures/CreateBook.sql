create procedure create_book @title NVARCHAR(255),
  @author NVARCHAR(255),
  @published_year int
as
begin
  set
nocount on;

insert into books (title, author, published_year)
values (@title, @author, @published_year)
end;