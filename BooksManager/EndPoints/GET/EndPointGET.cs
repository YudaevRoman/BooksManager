
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Azure;

namespace BooksManager
{
    public partial class Program
    {
        static public void MapGET_Root(string route)
        {
            app.MapGet(route, () => Results.Redirect("/home"));
        }
        static void MapGET_Authorization(string route)
        {
            app.MapGet(route, async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync("wwwroot/html/authorization.html");
            });
        }
        static void MapGET_Registration(string route)
        {
            app.MapGet(route, async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.SendFileAsync("wwwroot/html/registration.html");
            });
        }
        static void MapGET_Logout(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/authorization");
            });
        }
        static void MapGET_Home(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());

                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                htmlHome.GetTag("elementCollection").Tags.Clear();
                await htmlHome.AddContent_Home(htmlHome.GetTag("elementCollection"), person);

                if (person.Person_Image != null)
                {
                    htmlHome.GetTag("personImageUrl").Attributes["src"] = (await person.GetImage(dbManager)).Image_Path;
                }
                else
                {
                    htmlHome.GetTag("personImageUrl").Attributes["src"] = @"/image/default.jpg";
                }

                htmlHome.GetTag("personName").Value = person.Person_Name;
                htmlHome.GetTag("personRole").Value = (await person.GetRole(dbManager)).Role_Name;

                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(htmlHome.ToString());
            });
        }

        static void MapGET_HomeUserName(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context, string userName) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                Person user = await dbManager.Person.FirstOrDefaultAsync(p => p.Person_Name == userName);
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }
                context.Response.ContentType = "text/html; charset=utf-8";

                if (person == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Пользователь не найден");
                    return;
                }

                if (context.GetUserID() == person.Person_Id)
                {
                    context.Response.Redirect("/home");
                    return;
                }

                htmlUsername.GetTag("elementCollection").Tags.Clear();
                await htmlUsername.AddContent_UserName(htmlUsername.GetTag("elementCollection"), person);

                if (person.Person_Image != null)
                {
                    htmlUsername.GetTag("personImageUrl").Attributes["src"] = (await person.GetImage(dbManager)).Image_Path;
                }
                else
                {
                    htmlUsername.GetTag("personImageUrl").Attributes["src"] = @"/image/default.jpg";
                }
                htmlUsername.GetTag("userName").Value = user.Person_Name;
                htmlUsername.GetTag("userRole").Value = (await user.GetRole(dbManager)).Role_Name;
                htmlUsername.GetTag("personName").Value = person.Person_Name;

                await context.Response.WriteAsync(htmlUsername.ToString());
            });
        }
        static void MapGET_Books(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                htmlBooks.GetTag("elementCollection").Tags.Clear();
                await htmlBooks.AddContent_Books(htmlBooks.GetTag("elementCollection"));

                htmlBooks.GetTag("personName").Value = person.Person_Name;

                context.Response.ContentType = "text/html; charset=utf-8";

                await context.Response.WriteAsync(htmlBooks.ToString());
            });
        }
        static void MapGET_Book(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context, string id) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                Book book = await dbManager.Book.FindAsync(int.Parse(id));
                HtmlTag tag, tmp;
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                context.Response.ContentType = "text/html; charset=utf-8";

                if (book == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Книга не найдена");
                    return;
                }

                var image = await book.GetImage(dbManager);
                if (image != null) {
                    htmlBook.GetTag("bookImageUrl").Attributes["src"] = image.Image_Path;
                } else {
                    htmlBook.GetTag("bookImageUrl").Attributes["src"] = "/image/default.jpg";
                }
                htmlBook.GetTag("personName").Value = person.Person_Name;
                htmlBook.GetTag("bookName").Value = book.Book_Name;
                htmlBook.GetTag("bookDate").Value = book.Book_Date?.Date.Year.ToString();
                htmlBook.GetTag("bookPerson").Value =
                (await dbManager.Person.FindAsync((await dbManager.BooksAddedByPerson
                    .FirstOrDefaultAsync(b => b.BooksAddedByPerson_Book == book.Book_Id))
                    .BooksAddedByPerson_Person)).Person_Name;
                htmlBook.GetTag("description").Value = book.Book_Description;

                tag = htmlBook.GetTag("elementCollection_Categories");
                tmp = tag.Tags[0].Tags[0];
                tag.Tags[0].Tags.Clear();
                tag.Tags[0].Tags.Add(tmp);
                await htmlBook.AddContent_Book_Categories(tag, book);

                tag = htmlBook.GetTag("elementCollection_Authors");
                tmp = tag.Tags[0].Tags[0];
                tag.Tags[0].Tags.Clear();
                tag.Tags[0].Tags.Add(tmp);
                await htmlBook.AddContent_Book_Books(tag, book);

                await context.Response.WriteAsync(htmlBook.ToString());
            });
        }
        static void MapGET_Authors(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                htmlAuthors.GetTag("elementCollection").Tags.Clear();
                await htmlAuthors.AddContent_Authors(htmlAuthors.GetTag("elementCollection"));

                htmlAuthors.GetTag("personName").Value = person.Person_Name;

                context.Response.ContentType = "text/html; charset=utf-8"; ;

                await context.Response.WriteAsync(htmlAuthors.ToString());
            });
        }
        static void MapGET_Author(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context, string id) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                Author author = await dbManager.Author.FindAsync(int.Parse(id));
                HtmlTag tag, tmp;
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                context.Response.ContentType = "text/html; charset=utf-8";

                if (author == null)
                {
                    context.Response.StatusCode = 404;
                    await context.Response.WriteAsync("Автор не найден");
                    return;
                }

                var image = await author.GetImage(dbManager);
                if (image != null) {
                    htmlAuthor.GetTag("authorImageUrl").Attributes["src"] = image.Image_Path;
                } else {
                    htmlAuthor.GetTag("authorImageUrl").Attributes["src"] = "/image/default.jpg";
                }

                htmlAuthor.GetTag("personName").Value = person.Person_Name;
                htmlAuthor.GetTag("authorFirstName").Value = author.Author_FirstName;
                htmlAuthor.GetTag("authorLastName").Value = author.Author_LastName;

                tag = htmlAuthor.GetTag("elementCollection_Categories");
                tmp = tag.Tags[0].Tags[0];
                tag.Tags[0].Tags.Clear();
                tag.Tags[0].Tags.Add(tmp);
                await htmlAuthor.AddContent_Author_Categories(tag, author);

                tag = htmlAuthor.GetTag("elementCollection_Books");
                tmp = tag.Tags[0].Tags[0];
                tag.Tags[0].Tags.Clear();
                tag.Tags[0].Tags.Add(tmp);
                await htmlAuthor.AddContent_Author_Books(tag, author);

                await context.Response.WriteAsync(htmlAuthor.ToString());
            });
        }
        static void MapGET_Addedbook(string route)
        {
            app.MapGet(route, [Authorize] async (string? id, HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                HtmlTag tag, tmp;
                Book book;

                if (id != null) {
                    int numId;
                    if (int.TryParse(id, out numId)) {
                        book = await dbManager.Book.FindAsync(numId);
                        if (book == null) {
                            context.Response.StatusCode = 400;
                            Results.Text("Неверный идентификатора книги");
                            return;
                        }

                        string description = book.Book_Description;
                        tag = htmlAddedbook.GetTag("description");
                        tag.Attributes["value"] = description;

                        string bookName = book.Book_Name;
                        tag = htmlAddedbook.GetTag("bookName");
                        tag.Attributes["value"] = bookName;

                        string bookYear = book.Book_Date.Value.Year.ToString();
                        tag = htmlAddedbook.GetTag("bookYear");
                        tag.Attributes["value"] = bookYear;
                    } else {
                        context.Response.StatusCode = 400;
                        Results.Text("Неверный идентификатора книги");
                        return;
                    }
                } else {
                    book = new Book();
                }
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

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
            });
        }
        static void MapGET_Addedauthor(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                Person person = await dbManager.Person.FindAsync(context.GetUserID());
                
                if (person == null) {
                    context.Response.Redirect("/logout");
                    return;
                }

                context.Response.ContentType = "text/html; charset=utf-8";

                htmlAddedauthor.GetTag("personName").Value = person.Person_Name;

                await context.Response.WriteAsync(htmlAddedauthor.ToString());
            });
        }
        static void MapGET_Editor(string route)
        {
            app.MapGet(route, [Authorize]  async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(htmlHome.ToString());
            });
        }
        static void MapGET_EditorTable(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(htmlHome.ToString());
            });
        }
        static void MapGET_Admin(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(htmlHome.ToString());
            });
        }
        static void MapGET_AdminTable(string route)
        {
            app.MapGet(route, [Authorize] async (HttpContext context) => {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(htmlHome.ToString());
            });
        }
    }
}