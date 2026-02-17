# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-17

### Added
* **Authentication Module**:
    * Implemented `POST /api/auth/login` with JWT generation.
    * Integrated `BCrypt` for secure password hashing.
    * Created `UserSeeder` with initial accounts (Admin: `ADM001`, Student: `3120600001`, Prof: `19800101`).
* **User Schema**:
    * Established One-to-One relationships for `User` -> `Student` and `User` -> `Professor`.
    * Added Admin role authorization policies.
    * Added `GET /api/users/{id}` for Admin to view detailed user profiles.
* **Room Management (CRUD)**:
    * Implemented full CRUD endpoints for `Rooms`.
    * Added **Soft Delete** functionality (`IsDeleted` flag).
    * Added `RoomSeeder` with 5 initial dummy rooms.
* **Availability Search**:
    * Implemented logic to filter rooms by Date Range, Sector, and Capacity.
    * Added "Name Search" (partial match) capability.
    * **Exclusion Logic**: Rooms are hidden ONLY if they have overlapping `Approved` reservations.
* **Reservation System**:
    * Implemented `POST /api/reservations` for booking requests (Queue system).
    * Added **Anti-Spam Logic**: Users cannot re-apply for a room if they already have a pending request for that slot.
    * Added **Double Booking Protection**: Blocks requests if an `Approved` reservation exists.
* **Admin Approval Workflow**:
    * Implemented `PATCH /api/reservations/{id}/status`.
    * **Feature**: Cascading Rejection - Approving a reservation automatically rejects all conflicting pending requests.
    * Implemented `PUT /api/reservations/{id}` for manual Admin corrections.
    * Implemented `GET /api/reservations/{id}` for detailed viewing.
* **Infrastructure**:
    * configured `osaka.yml` for Dockerized SQL Server 2022 setup.