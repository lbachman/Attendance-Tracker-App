# Student Attendance Tracker

A comprehensive student attendance tracking system that features a responsive web interface, a mobile application, and a robust backend to manage data efficiently. The project utilizes modern web, mobile, and backend technologies to deliver a seamless and efficient experience.

## Components

### 1. Frontend (Web)

- **Framework**: React
- **Description**: Provides an intuitive and user-friendly interface for managing and viewing student attendance records.
- **Key Features**:
  - Dashboard displaying attendance statistics
  - Ability to mark attendance and generate reports
  - Filtering and sorting of student data

### 2. Frontend (Mobile)

- **Framework**: .NET MAUI (Multi-platform App UI)
- **Description**: Cross-platform mobile application for both Android and iOS, offering similar features as the web frontend.
- **Key Features**:
  - Attendance marking and viewing on the go
  - Notifications for absences or alerts
  - Offline access and synchronization with the main database

### 3. Backend

- **Framework**: ASP.NET Core
- **Data Management**: Entity Framework Core with ASP.NET Identity for authentication and authorization
- **Description**: Provides secure APIs for handling attendance data and user management.
- **Key Features**:
  - RESTful API endpoints for CRUD operations on student records
  - Authentication and authorization using ASP.NET Identity
  - Database management using Entity Framework Core

## Setup Instructions

1. **Backend**:
   - Clone the repository.
   - Navigate to the `backend` directory.
   - Configure the database connection in `appsettings.json`.
   - Run database migrations using Entity Framework Core.
   - Start the backend server.

2. **Web Frontend**:
   - Navigate to the `web-frontend` directory.
   - Install dependencies with `npm install`.
   - Configure API endpoints in `.env` or `config.js`.
   - Start the development server using `npm start`.

3. **Mobile Frontend**:
   - Navigate to the `mobile-frontend` directory.
   - Open the solution in Visual Studio.
   - Configure the API endpoint in the code.
   - Build and run on the desired device or emulator.

## Contribution Guidelines

Contributions are welcome! Please follow the guidelines outlined in the `CONTRIBUTING.md` file.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more details.

