\# User Management API



A simple ASP.NET Core Web API created for creating, retrieving, updating, and deleting users.



The project was developed as part of a three-stage activity focused on:

\- Building CRUD API endpoints

\- Debugging and improving the API with Microsoft Copilot

\- Adding middleware for logging, error handling, and token-based authentication



\## Features



\- Get all users

\- Get a user by ID

\- Create a new user

\- Update an existing user

\- Delete a user

\- User input validation

\- Pagination

\- Error handling

\- Request/response logging

\- Token-based authentication



\## Technologies



\- ASP.NET Core

\- .NET

\- Minimal APIs

\- Microsoft Copilot



\## API Endpoints



| Method | Endpoint | Description |



| GET | `/users` | Get all users |

| GET | `/users/{id}` | Get a user by ID |

| POST | `/users` | Create a new user |

| PUT | `/users/{id}` | Update an existing user |

| DELETE | `/users/{id}` | Delete a user |



\## Validation



The API validates user input before processing requests.



\- Name is required

\- Email is required

\- Email must have a valid format

\- Invalid data returns `400 Bad Request`



\## Authentication



The API uses simple token-based authentication.



Requests must include an `Authorization` header with a Bearer token.

Missing or invalid tokens return `401 Unauthorized`.



\## Middleware



The API includes the following middleware:



1\. Error handling middleware

2\. Token authentication middleware

3\. Request/response logging middleware



\## Error Handling



Unhandled exceptions are handled by centralized middleware.

The API returns `500 Internal Server Error` with a consistent JSON response:



\## Logging



The logging middleware records:



\- HTTP method

\- Request path

\- Response status code



\## How Microsoft Copilot Was Used



Microsoft Copilot was used throughout the development process.



Copilot helped review and improve the initial ASP.NET Core API setup.



\### CRUD Development



I first created the basic CRUD functionality and then used Copilot to review the implementation.



Copilot helped me identify issues such as:



\- Incorrect ID handling

\- Missing validation

\- Incorrect behavior when users did not exist

\- Possible improvements to HTTP status codes



\### Debugging



Copilot helped identify and fix:



\- Invalid user input handling

\- Email validation

\- Error handling

\- Performance improvements for user lookup



The original implementation used a list and relied on indexes for user lookup. The implementation was improved to use ID-based lookup instead.



\### Middleware



Copilot also helped me create and review:



\- Error handling middleware

\- Token authentication middleware

\- Request/response logging middleware



\## Testing



The API was tested with HTTP requests for:



\- Valid and invalid authentication tokens

\- Missing authentication token

\- Existing users

\- Non-existent users

\- Valid user creation

\- Invalid email addresses

\- Empty fields

\- User updates

\- User deletion

\- Invalid pagination

\- Unhandled exceptions





