create procedure get_pagination_books @page int,
  @page_size int
as
begin
  set
nocount on;

select title,
       author,
       published_year
from books
order by id
offset (@page - 1)*@page_size rows fetch next @page_size rows only;
end;