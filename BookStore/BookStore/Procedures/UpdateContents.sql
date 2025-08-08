create procedure update_contents @id int,
  @contents xml
as
begin
  set
nocount on;

update books
set table_of_contents = @contents
WHERE id = @id;
end;