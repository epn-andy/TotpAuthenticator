# TotpAuthenticator

This project is a .NET Core web API that provides endpoints for generating and validating Time-Based One-Time Passwords (TOTP). It uses the `Otp.NET` library for TOTP generation and validation and `QRCoder` to generate QR codes for easy setup with authenticator apps.

## Getting Started

### Prerequisites

*   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Installation

1.  Clone the repository:

    ```bash
    git clone https://github.com/your-username/TotpAuthenticator.git
    ```

2.  Navigate to the project directory:

    ```bash
    cd TotpAuthenticator
    ```

3.  Build the project:

    ```bash
    dotnet build
    ```

4.  Run the project:

    ```bash
    dotnet run
    ```

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

## API Endpoints

### POST /generate-qr

This endpoint generates a QR code for setting up TOTP authentication.

**Request Body:**

```json
{
  "userId": "your-unique-user-id"
}
```

**Response:**

*   **200 OK:** Returns a PNG image of the QR code.
*   **400 Bad Request:** If the `userId` is missing or already registered.

### POST /verify-totp

This endpoint verifies a TOTP code.

**Request Body:**

```json
{
  "userId": "your-unique-user-id",
  "totpCode": "123456"
}
```

**Response:**

*   **200 OK:** Returns a JSON object with a boolean `isValid` property.

    ```json
    {
      "isValid": true
    }
    ```

*   **400 Bad Request:** If the `userId` or `totpCode` is missing or invalid.
