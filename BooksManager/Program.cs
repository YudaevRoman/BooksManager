using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BooksManager
{
    public partial class Program
    {
        //html �������� (��������� ������������� � ������, ����� ����������� �� ��������)
        static HtmlDocument htmlHome;
        static HtmlDocument htmlBook;
        static HtmlDocument htmlBooks;
        static HtmlDocument htmlAuthor;
        static HtmlDocument htmlAuthors;
        static HtmlDocument htmlUsername;
        static HtmlDocument htmlAddedbook;
        static HtmlDocument htmlAddedauthor;

        //������ ���� ������
        static DbBooksManager dbManager;

        //������ ��������� ����������
        static WebApplicationBuilder builder;

        //������ ����������
        static WebApplication app;
        
        //������� ����������
        static string currentDirectory;

        static void Main(string[] args)
        {
            currentDirectory = Directory.GetCurrentDirectory();

            htmlHome = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/home.html");
            htmlBook = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/book.html");
            htmlBooks = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/books.html");
            htmlAuthor = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/author.html");
            htmlAuthors = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/authors.html");
            htmlUsername = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/username.html");
            htmlAddedbook = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/addedbook.html");
            htmlAddedauthor = HtmlHelper.FromHtmlFile($"{currentDirectory}/wwwroot/html/addedauthor.html");

            dbManager = new DbBooksManager(); 

            builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option => {
                        option.LoginPath = "/authorization";
                        option.AccessDeniedPath = "/home";
                    }
                );
            builder.Services.AddAuthorization();

            app = builder.Build();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();

            //������� �� �������� ��� ���� �������
            MapGET_Root("/");
            MapGET_Authorization("/authorization");
            MapGET_Registration("/registration");
            MapGET_Logout("/logout");
            MapGET_Home("/home");
            MapGET_HomeUserName("/home/{userName}");
            MapGET_Books("/books");
            MapGET_Book("/books/{id}");
            MapGET_Authors("/authors");
            MapGET_Author("/authors/{id}");
            MapGET_Addedbook("/addedbook/{id?}");
            MapGET_Addedauthor("/addedauthor");
            MapGET_Editor("/editor");
            MapGET_EditorTable("/editor/{tables}");
            MapGET_Admin("/admin");
            MapGET_AdminTable("/admin/{table}");

            //������� �� ��������� ������
            MapPOST_Authorization("/authorization");
            MapPOST_Registration("/registration");
            MapPOST_Addedbook("/addedbook/{id?}");
            MapPOST_Addedauthor("/addedauthor");
            MapPOST_Deleted("addedbook/deleted/{id}");

            ////��������� �������������� �����
            //app.Use(async (context, next) => {
            //    await next.Invoke();
            //    if (context.Response.StatusCode == 404) {
            //        Results.Redirect("/home");
            //    }
            //});

            app.Run();
        }
    }
}
