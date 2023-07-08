
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace BooksManager {
    public partial class Program
    {
        static void MapPOST_Authorization(string route)
        {
            app.MapPost(route, async (string? returnUrl, HttpContext context) => {
                string personName = context.Request.Form["personName"];
                string personPassword = context.Request.Form["personPassword"];
                Person person;

                if (personName == string.Empty) {
                    context.Response.StatusCode = 400;
                    return Results.Text("Укажите логин");
                }

                person = await dbManager.Person.FirstOrDefaultAsync(p =>
                    p.Person_Name == personName &&
                    p.Person_Password == personPassword
                    );

                if (person == null) {
                    context.Response.StatusCode = 400;
                    return Results.Text("Неверный логин и/или пароль");
                }

                await AddPersonCookies(context, person);

                return Results.Redirect(returnUrl ?? "/home");
            });
        }
        static void MapPOST_Registration(string route)
        {
            app.MapPost(route, async (string? returnUrl, HttpContext context) => {
                string personName = context.Request.Form["personName"];
                string personPassword = context.Request.Form["personPassword"];
                Person person;

                var file = context.Request.Form.Files.FirstOrDefault();

                if (personName == string.Empty) {
                    context.Response.StatusCode = 400;
                    return Results.Text("Укажите логин");
                }

                if (await dbManager.Person.FirstOrDefaultAsync(person => person.Person_Name == personName) != null)
                {
                    context.Response.StatusCode = 400;
                    return Results.Text("Данный логин уже занят");
                }

                person = new Person();

                person.Person_Name = personName;
                person.Person_Password = personPassword;
                person.PersonRole = await dbManager.PersonRole.FindAsync(2);
                person.Person_Role = person.PersonRole.Role_Id ?? 2;

                if (file != null) {
                    await dbManager.ImageFile.AddAsync(new ImageFile() { Image_Path = "image" });
                    await dbManager.SaveChangesAsync();

                    ImageFile imageFile = await dbManager.ImageFile.FirstOrDefaultAsync(i => i.Image_Path == "image");
                    imageFile.Image_Path = $"/image/persons/{imageFile.Image_Id}{Path.GetExtension(file.FileName)}";

                    person.ImageFile = imageFile;
                    person.Person_Image = imageFile.Image_Id;

                    await file.CopyToAsync(new FileStream($"{currentDirectory}/wwwroot{imageFile.Image_Path}", FileMode.Create));
                }

                await dbManager.Person.AddAsync(person);
                await dbManager.SaveChangesAsync();

                await AddPersonCookies(context, person);

                return Results.Redirect(returnUrl ?? "/home");
            });
        }
        static async Task AddPersonCookies(HttpContext context, Person person)
        {
            var claims = new List<Claim> {
                    new Claim("ID", person.Person_Id.ToString()),
                    new Claim(ClaimTypes.Name, person.Person_Name),
                    new Claim(ClaimTypes.Role, (await person.GetRole(dbManager)).Role_Name)
                };

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
                );

            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
                );
        }

        static void MapPOST_Addedbook(string route)
        {
            app.MapPost(route, [Authorize] async (string? returnUrl, string? id, HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                string categoryName, bookName, bookYear, description, checkAdd;
                var file = context.Request.Form.Files.FirstOrDefault();
                string check = context.Request.Form["checkButton"];
                List<int> categories = new List<int>();
                List<int> authors = new List<int>();
                Book book;

                if (id != null) {
                    int numId;
                    if (int.TryParse(id, out numId)) {
                        book = await dbManager.Book.FindAsync(numId);
                        if (book == null) {
                            context.Response.StatusCode = 400;
                            return Results.Text("Неверный идентификатора книги");
                        }
                    } else {
                        context.Response.StatusCode = 400;
                        return Results.Text("Неверный идентификатора книги");
                    }
                } else {
                    book = new Book();
                }

                if (check == "author")
                {
                    return Results.Redirect("/addedauthor?ReturnUrl=%2Faddedbook", false);
                }

                if (check == "category")
                {
                    categoryName = context.Request.Form["categoryName"];
                    Category category = new Category();

                    if (categoryName == null)
                    {
                        context.Response.StatusCode = 400;
                        return Results.Text("Укажите название категории");
                    }

                    category.Category_Name = categoryName;

                    await dbManager.Category.AddAsync(category);
                    await dbManager.SaveChangesAsync();
                    await UpdateAddbook(context);

                    return Results.Ok();
                }

                if (check == "addBook")
                {
                    bookName  = context.Request.Form["bookName"];
                    bookYear = context.Request.Form["bookYear"];
                    description = context.Request.Form["description"];

                    if (bookName == null)
                    {
                        context.Response.StatusCode = 400;
                        return Results.Text("Укажите название книги");
                    }
                    if (bookYear == null)
                    {
                        context.Response.StatusCode = 400;
                        return Results.Text("Укажите год издания книги");
                    }

                    await dbManager.Category.ForEachAsync(category =>
                    {
                        checkAdd = context.Request.Form[$"_category{category.Category_Id}_"];
                        if (checkAdd != string.Empty && checkAdd == "true")
                        {
                            categories.Add(category.Category_Id??0);
                        }
                    });
                    await dbManager.Author.ForEachAsync(author =>
                    {
                        checkAdd = context.Request.Form[$"_author{author.Author_Id}_"];
                        if (checkAdd != string.Empty && checkAdd == "true")
                        {
                            authors.Add(author.Author_Id??0);
                        }
                    });

                    if (categories.Count == 0)
                    {
                        context.Response.StatusCode = 400;
                        return Results.Text("У книги доллжна быть хотя бы одна категория");
                    }

                    if (file != null)
                    {
                        await dbManager.ImageFile.AddAsync(new ImageFile() { Image_Path = "image" });
                        await dbManager.SaveChangesAsync();

                        ImageFile imageFile = await dbManager.ImageFile.FirstOrDefaultAsync(i => i.Image_Path == "image");
                        imageFile.Image_Path = $"/image/books/{imageFile.Image_Id}{Path.GetExtension(file.FileName)}";

                        book.ImageFile = imageFile;
                        book.Book_Image = imageFile.Image_Id;

                        await file.CopyToAsync(new FileStream($"{currentDirectory}/wwwroot{imageFile.Image_Path}", FileMode.Create));
                    }

                    book.Book_Name = "-name-book-get-id-";
                    book.Book_Date = new DateTime(int.Parse(bookYear), 1, 1);
                    book.Book_Description = description;
                    await dbManager.Book.AddAsync(book);
                    await dbManager.SaveChangesAsync();

                    book.Book_Name = bookName;
                    BooksCategory booksCategory;
                    foreach (var category in categories)
                    {
                        booksCategory = new BooksCategory();
                        booksCategory.BooksCategory_Book = book.Book_Id;
                        booksCategory.BooksCategory_Category = category;
                        await dbManager.BooksCategory.AddAsync(booksCategory);
                    }
                    AuthorBooks authorBooks;
                    foreach (var author in authors)
                    {
                        authorBooks = new AuthorBooks();
                        authorBooks.AuthorBooks_Book = book.Book_Id;
                        authorBooks.AuthotBooks_Author = author;
                        await dbManager.AuthorBooks.AddAsync(authorBooks);
                    }

                    BooksAddedByPerson booksAddedByPerson = new BooksAddedByPerson();
                    booksAddedByPerson.BooksAddedByPerson_Person = person.Person_Id??0;
                    booksAddedByPerson.BooksAddedByPerson_Book = book.Book_Id;
                    await dbManager.BooksAddedByPerson.AddAsync(booksAddedByPerson);

                    await dbManager.SaveChangesAsync();

                    return Results.Redirect(returnUrl ?? "/home");
                }

                return Results.Ok();
            });
        }
        static void MapPOST_Addedauthor(string route)
        {
            app.MapPost(route, [Authorize] async (string? returnUrl, string? id, HttpContext context) => {
                string authorName = context.Request.Form["authorName"];
                string authorLastName = context.Request.Form["authorLastName"];
                Author author = new Author();

                var file = context.Request.Form.Files.FirstOrDefault();

                if (authorName == string.Empty)
                {
                    context.Response.StatusCode = 400;
                    return Results.Text("Укажите имя автора");
                }

                if (authorLastName == string.Empty)
                {
                    context.Response.StatusCode = 400;
                    return Results.Text("Укажите фамилию автора");
                }

                author.Author_FirstName = authorName;
                author.Author_LastName = authorLastName;

                if (file != null)
                {
                    await dbManager.ImageFile.AddAsync(new ImageFile() { Image_Path = "image" });
                    await dbManager.SaveChangesAsync();

                    ImageFile imageFile = await dbManager.ImageFile.FirstOrDefaultAsync(i => i.Image_Path == "image");
                    imageFile.Image_Path = $"/image/authors/{imageFile.Image_Id}{Path.GetExtension(file.FileName)}";

                    author.ImageFile = imageFile;
                    author.Author_Image = imageFile.Image_Id;

                    await file.CopyToAsync(new FileStream($"{currentDirectory}/wwwroot{imageFile.Image_Path}", FileMode.Create));
                }

                await dbManager.Author.AddAsync(author);
                await dbManager.SaveChangesAsync();

                return Results.Redirect(returnUrl ?? "/home");
            });
        }
        static async Task UpdateAddbook(HttpContext context)
        {
            Person person = await dbManager.Person.FindAsync(context.GetUserID());
            HtmlTag tag, tmp;

            string description = context.Request.Form["description"];
            tag = htmlAddedbook.GetTag("description");
            tag.Attributes["value"] = description;

            string bookName = context.Request.Form["bookName"];
            tag = htmlAddedbook.GetTag("bookName");
            tag.Attributes["value"] = bookName;

            string bookYear = context.Request.Form["bookYear"];
            tag = htmlAddedbook.GetTag("bookYear");
            tag.Attributes["value"] = bookYear;

            tag = htmlAddedbook.GetTag("elementCollection_Categories");
            tmp = tag.Tags[0].Tags[0];
            tag.Tags[0].Tags.Clear();
            tag.Tags[0].Tags.Add(tmp);
            await htmlAddedbook.AddContent_Addedbook_Categories(tag);

            tag = htmlAddedbook.GetTag("elementCollection_Authors");
            tmp = tag.Tags[0].Tags[0];
            tag.Tags[0].Tags.Clear();
            tag.Tags[0].Tags.Add(tmp);
            await htmlAddedbook.AddContent_Addedbook_Authors(tag);

            htmlAddedbook.GetTag("personName").Value = person.Person_Name;

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(htmlAddedbook.ToString());
        }
        static void MapPOST_Deleted(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context, string id) =>
            {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                Book book = await dbManager.Book.FindAsync(int.Parse(id));

                context.Response.ContentType = "text/html; charset=utf-8";

                if (book == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Книга не найдена");
                    return;
                }

                dbManager.Book.Remove(book);
                await dbManager.SaveChangesAsync();

                context.Response.Redirect("/home");
            });
        }
    }
}