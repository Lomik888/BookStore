create procedure get_book @book_id int
as
begin
  set
nocount on;

select title,
       author,
       published_year
from books
where id = @book_id;
end;