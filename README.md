# NG ASP Forum

## What is this?
NG ASP Forum is a simple forum implementation based on Angular and ASP.NET, using Entity Framework and SQLite3 for database operations

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
```

## Building and Hosting
At the time of writing, there is no intended method for building and hosting, as the implementation is still in development