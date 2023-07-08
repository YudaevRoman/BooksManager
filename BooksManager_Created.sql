create database BooksManager
go
use BooksManager
go
create table PersonRole (
	Role_Id int primary key identity(0, 1),
	Role_Name nvarchar(20) not null
)
go
insert into PersonRole values('admin')
insert into PersonRole values('editor')
insert into PersonRole values('user')
go
create table ImageFile (
	Image_Id int primary key identity(0, 1),
	Image_Path nvarchar(100) not null
)
create table Person (
	Person_Id int primary key identity(0, 1),
	Person_Role int not null,
	Person_Name nvarchar(20) unique not null,
	Person_Password nvarchar(20),
	Person_Image int,
	constraint FK_Person_K_PersonRole
		foreign key (Person_Role) 
		references PersonRole(Role_Id)
			on delete cascade,
	constraint FK_Person_K_ImageFile
		foreign key (Person_Image) 
		references ImageFile(Image_Id)
			on delete set null
)
go
insert into Person values(
	(select Role_Id 
		from PersonRole 
		where Role_Name = 'admin'), 
		'admin',
		'admin',
		null)
go
create table Book (
	Book_Id int primary key identity(0, 1),
	Book_Name nvarchar(100) not null,
	Book_Date date,
	Book_Description nvarchar(512),
	Book_Image int,
	constraint FK_Book_K_ImageFile
		foreign key (Book_Image) 
		references ImageFile(Image_Id)
			on delete set null
)
create table Category (
	Category_Id int primary key identity(0, 1),
	Category_Name nvarchar(20) unique not null
)
create table BooksCategory (
	BooksCategory_Id int primary key identity(0, 1),
	BooksCategory_Book int not null,
	BooksCategory_Category int not null,
	constraint FK_BooksCategory_K_Book
		foreign key (BooksCategory_Book) 
		references Book(Book_Id)
			on delete cascade,
	constraint FK_BooksCategory_K_Category
		foreign key (BooksCategory_Category) 
		references Category(Category_Id)
			on delete cascade
)
create table BooksAddedByPerson (
	BooksAddedByPerson_Id int primary key identity(0, 1),
	BooksAddedByPerson_Person int not null,
	BooksAddedByPerson_Book int not null,
	constraint FK_BooksAddedByPerson_K_Person
		foreign key (BooksAddedByPerson_Person)
		references Person(Person_Id)
			on delete cascade,
	constraint FK_BooksAddedByPerson_K_Book
		foreign key (BooksAddedByPerson_Book)
		references Book(Book_Id)
			on delete cascade
)
create table Author (
	Author_Id int primary key identity(0, 1),
	Author_LastName nvarchar(20) not null,
	Author_FirstName nvarchar(20),
	Author_Image int, 
	constraint FK_Author_K_ImageFile
		foreign key (Author_Image) 
		references ImageFile(Image_Id)
			on delete set null
)
create table AuthorBooks (
	AuthorBooks_Id int primary key identity(0,1),
	AuthotBooks_Author int not null,
	AuthorBooks_Book int not null,
	constraint FK_AuthorBooks_K_Author
		foreign key (AuthotBooks_Author)
		references Author(Author_Id)
			on delete cascade,
	constraint FK_AuthorBooks_K_Book
		foreign key (AuthorBooks_Book)
		references Book(Book_Id)
			on delete cascade
)