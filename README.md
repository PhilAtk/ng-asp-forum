# NG ASP Forum

## What is this?
NG ASP Forum is a simple forum implementation based on Angular and ASP.NET, using Entity Framework and MySQL for database operations

At the time of writing it is NOT recommended to be used as an actual forum. It is far from feature complete, and I cannot guarantee the usability or security of it as this time.

# How to Use

## Configuration
Before attempting to run, the following environment variables must be set:
```
    EMAIL_USERNAME # username for an SMTP server
    EMAIL_PASSWORD # password for an SMTP server
    EMAIL_SERVER # the SMTP server to use
    AUTH_ISSUER # Issuer for Auth Tokens
    AUTH_AUDIENCE # Audience for Auth Tokens
    AUTH_SECRET # Secret for encryption

    ConnectionStrings__DefaultConnection # e.g. 'server=localhost;user=root;password=your_pass;database=ef'

    MYSQL_ROOT_PASSWORD # your_pass, if using the example connection string
    MYSQL_DATABASE # ef, if using the example connection string
```

## Building and Hosting
The simplest way to run the forum is to use the provided docker-compose.yml file. To use:

1. Clone this repository with git clone
2. Set the above environment variables in a .env file to load in
3. Run `docker compose --env-file [your-env-file.env] up`

If all the required environment variables are set, the MySQL server and Forum instance should be running.

By default, there are no forum users. Register a user by following the prompts on the application page.

It is expected that a SysOp should be able to connect to the MySQL server to set their role to SysOp through database operations.
At time of writing, there is no in-forum solution to set users as Admin.