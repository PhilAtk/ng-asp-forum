# NG ASP Forum

## What is this?
NG ASP Forum is a simple forum implementation based on Angular and ASP.NET, using Entity Framework and SQLite3 for database operations

At the time of writing it is NOT recommended to be used as an actual forum. It is far from feature complete, and I cannot guarantee the usability or security of it as this time.

# How to Use

## Configuration
Before attempting to run, the following environment variables must be set:
```
    email:username # username for an SMTP server
    email:password # password for an SMTP server
    email:mailServer # the SMTP server to use
    auth:issuer # Issuer for Auth Tokens
    auth:audience # Audience for Auth Tokens
    auth:secret # Secret for encryption
```

## Building and Hosting
At the time of writing, there is no intended method for building and hosting, as the implementation is still in development