# READ ME #

When using MySql connector with Entity Framework and ASP.NET Identity, you may encounter the error
````
Specified key was too long; max key length is 767 bytes
````

## Reason ##

The error was cause by the fact that the database was created using utf8 collation.
MySql or MariaDB store UTF8 string using 3 bytes per character plus 2 bytes for length.

So a varchar(256) would require 770 bytes, which just exceed the max key length of 767 bytes

Most solution I came across tell you to reduce the key length, however, it is unacceptable for field that store email address like ASP.NET IdentityUser because the official specification for email address is 256 ansi characters.

## Solution ##
1. Tell EF that we're not using Unicode for Username and Email in IdentityUser
2. Reduce the string length of Name property for IdentityRole. This is acceptable as role name doesn't need to be that long.



3.Override MySql.Data.Entity.MySqlMigrationSqlGenerator class to tell it to use latin1_general_ci for collation.

## Usage ##

1. Add the following code to your OnModelCreating method

````
protected override void OnModelCreating(DbModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // email address doesn't need to be in unicode, check it spec
    modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).IsUnicode(false);
    modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).IsUnicode(false);
    modelBuilder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(255);
}
````

2. Add the following code to your entityFramework element in web.config or app.config file

````
<entityFramework codeConfigurationType="MySql.AspNetIdentity.MySqlEFConfiguration, MySql.AspNetIdentity">
````

## Build ##

To build this solution, you need to add reference to MySql connector.

My SQL connector can be download at [http://dev.mysql.com/downloads/connector/net/]()
