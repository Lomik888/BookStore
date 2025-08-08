create procedure update_book_info_by_id @id int,
  @title NVARCHAR(255) = NULL,
  @publish_year int = NULL
as
begin
  set
nocount on;

update books
set title          = COALESCE(@title, title),
    published_year = COALESCE(@publish_year, published_year)
where id = @id;
end;