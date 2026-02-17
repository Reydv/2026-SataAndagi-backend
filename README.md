# 2026-SataAndagi-backend

## Description
The Backend service for the **Sata Andagi** facility management system. This system allows Students and Professors to search for available rooms and submit booking requests. It features a "Queue-based" reservation system where multiple users can request the same slot, subject to Admin approval with automatic conflict resolution.

## Features
* **Authentication & Authorization**: Secure Login (JWT) with Role-based access (Admin, Student, Professor).
* **Master Room Management**: Full CRUD for facility rooms with Soft Delete capabilities (`DeletedAt` timestamp).
* **Smart Availability Search**:
    * Filter by Date, Sector, Capacity, and Room Name.
    * Excludes rooms only if they have **Approved** bookings (Overlap logic).
* **Reservation System (Queue Logic)**:
    * Users can submit requests (Status: Pending).
    * **Double Booking Protection**: Blocks requests if the user already has a pending/approved request for the same slot.
    * **Cascading Rejection**: When an Admin **Approves** a request, all conflicting Pending requests are automatically **Rejected**.
* **Admin Dashboard**:
    * View all booking history with pagination and status filters.
    * Manual correction endpoints for fixing data errors.

## Tech Stack
* **Framework**: ASP.NET Core 10.0
* **Language**: C#
* **Database**: SQL Server 2022 (Docker Container)
* **Authentication**: JWT Bearer Authentication
* **Security**: BCrypt.Net-Next (Password Hashing)
* **Tools**: Swagger UI, Dotnet CLI, Docker

## Prerequisites
* [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
* [Docker Desktop](https://www.docker.com/products/docker-desktop)
* Git

## Installation

1.  **Clone the Repository**
    ```bash
    git clone [https://github.com/pens-pbl/2026-SataAndagi-backend.git](https://github.com/pens-pbl/2026-SataAndagi-backend.git)
    cd 2026-SataAndagi-backend
    ```

2.  **Start Database Container**
    This project uses `osaka.yml` for the database configuration.
    ```bash
    docker compose -f osaka.yml up -d
    ```

3.  **Configure Environment**
    Update `src/appsettings.json` with the credentials below.
    *(See [Environment Variables](#environment-variables) section below)*.

4.  **Restore Dependencies**
    ```bash
    dotnet restore
    ```

5.  **Run Migrations**
    Apply the database schema (Users, Students, Professors, Rooms, Reservations).
    ```bash
    dotnet ef database update
    ```

6.  **Seed Database**
    The application automatically seeds initial data (Admin account, Dummy Rooms) on startup.
    ```bash
    dotnet run --project src/2026-SataAndagi-backend.csproj
    ```

## Usage

### Starting the Server
```bash
dotnet run --project src/2026-SataAndagi-backend.csproj
```
The API will be available at `http://localhost:5xxx` (or similar).
Swagger Documentation: `http://localhost:5xxx/swagger`

## API Endpoints

| Module | Method | Endpoint | Description | Attributes |
| :--- | :--- | :--- | :--- | :--- |
| **Auth** | POST | `/api/auth/login` | Login and receive JWT. | **Body:**<br>`IdentityNumber`<br>`Password` |
| **Users** | GET | `/api/users/{id}` | **(Admin)** View User Details. | **Path:** `id` |
| **Rooms** | GET | `/api/rooms/availability` | Search rooms. | **Query:**<br>`startDate`<br>`endDate`<br>`sector`<br>`minCapacity`<br>`search` |
| | GET | `/api/rooms` | List all rooms. | |
| | GET | `/api/rooms/{id}` | Get room details. | **Path:** `id` |
| | POST | `/api/rooms` | **(Admin)** Create a new room. | **Body:**<br>`Name`<br>`Sector`<br>`Capacity` |
| | PUT | `/api/rooms/{id}` | **(Admin)** Update room details. | **Body:**<br>`Name`<br>`Sector`<br>`Capacity`<br>`IsAvailable` |
| | DELETE | `/api/rooms/{id}` | **(Admin)** Soft delete a room. | **Path:** `id` |
| **Reservations** | POST | `/api/reservations` | Submit a booking request. | **Body:**<br>`RoomId`<br>`StartTime`<br>`EndTime`<br>`Purpose` |
| | GET | `/api/reservations` | View history. | **Query:**<br>`page`<br>`pageSize`<br>`search`<br>`status`<br>`dates` |
| | GET | `/api/reservations/{id}` | View reservation detail. | **Path:** `id` |
| | DELETE | `/api/reservations/{id}` | Cancel a pending request. | **Path:** `id` |
| | PATCH | `/api/reservations/{id}/status` | **(Admin)** Approve/Reject. | **Body:**<br>`Status` ("Approved"/"Rejected") |
| | PUT | `/api/reservations/{id}` | **(Admin)** Correction Update. | **Body:** All fields (Manual override) |

## Environment Variables
Ensure your `src/appsettings.json` matches the `osaka.yml` configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=SataAndagiDb;User Id=sa;Password=uncrackable_123A;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "SataAndagiBackend",
    "Audience": "SataAndagiClients"
  }
}
```

## Contributing

1.  **Branching Strategy**
    * `main`: Production-ready code.
    * `develop`: Integration branch.
    * `feature/<name>`: New features.
    * `fix/<name>`: Bug fixes.

2.  **Commit Messages**
    We follow **Conventional Commits**:
    * `feat: add room search logic`
    * `fix: resolve db connection timeout`
    * `docs: update readme with endpoints`

3.  **Workflow**
    1.  Create a branch (`git checkout -b feature/AmazingFeature`).
    2.  Commit your changes.
    3.  Push to the branch (`git push origin feature/AmazingFeature`).
    4.  Open a Pull Request to `develop`.

## License
Distributed under the MIT License.

## Authors
* **PBL Team 2026** - *Initial Work*