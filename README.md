# Fitness Pro

The Fitness Pro App is a comprehensive web application designed to manage gym branches, trainers, classes, and online training programs. It incorporates advanced features like role-based authorization, real-time notifications, secure payment processing, and cloud storage, ensuring an efficient and seamless user experience for admins, trainers, and trainees.

## Key Features
- **CRUD Operations**: Effortlessly create, read, update, and delete gyms, trainers, classes, subscriptions, and users.
- **Filtering, Sorting, Searching, and Pagination**: Optimized performance with advanced query capabilities for easy data management and discovery.
- **Authentication and Authorization**: Secure user authentication using JWT tokens, with role-based authorization to control access.
- **Real-Time Notifications**: Receive live updates and notifications using SignalR.
- **Payment Integration**: Seamlessly process payments through Stripe for online training subscriptions.
- **Azure Blob Storage Integration**: Efficient and secure image and file uploads for gym logos, trainer photos, and more.

## Technologies Used
- **.NET 8**: Backend built with .NET 8 APIs, providing scalability and performance.
- **Entity Framework Core**: Used for data access and management, with full support for LINQ and relational mapping.
- **SignalR**: Enables real-time communication and live updates between client and server.
- **Stripe API**: Integrated for secure and reliable payment processing.
- **Azure Blob Storage**: Used to handle cloud-based file storage.
- **SQL Server**: Main relational database for business data.
- **Clean Architecture**: Separation of concerns to improve scalability, modularity, and maintainability.
- **Clean Code Principles**: Codebase follows SOLID, DRY, and KISS principles for maintainable and readable code.

## Additional Technical Highlights
- **JWT Authentication**: Token-based authentication ensures secure, stateless user sessions.
- **Generic Repository Pattern**: A reusable and type-safe approach to data access logic across entities.
- **Dependency Injection**: Promotes loose coupling and testability.
- **Comprehensive Error Handling**: Provides user-friendly error messages and robust exception management.
- **Cross-Origin Resource Sharing (CORS)**: Configured for secure communication between client and server.

## Installation and Setup
1. Clone the repository:
    ```bash
    git clone https://github.com/Ahmed-Refa3e/Fitness-Pro.git
    ```
2. Navigate to the project directory:
    ```bash
    cd Fitness-Pro
    ```
3. Install the dependencies:
    ```bash
    dotnet restore
    ```
4. Set up the database and environment variables:
    ```bash
    # Create a .env file and add your environment-specific credentials
    DATABASE_URL=your_database_url
    STRIPE_SECRET_KEY=your_stripe_secret_key
    AZURE_STORAGE_CONNECTION_STRING=your_azure_storage_connection_string
    JWT_SECRET_KEY=your_jwt_secret_key
    ```
5. Run the application:
    ```bash
    dotnet run
    ```

## Usage
- Access the API documentation through your browser at `https://fitnesspro.runasp.net/swagger/index.html`.
- Use Swagger UI to interact with endpoints for managing gyms, trainers, classes, subscriptions, and users, with real-time notifications and secure payment and storage features.

## Contributing
Contributions are welcome! Please open an issue or submit a pull request for any improvements or bug fixes.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

## Acknowledgements
Special thanks to the open-source community for providing the tools and frameworks used in this project.

---

<div align="left">
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/csharp/csharp-original.svg" height="40" alt="csharp logo"  />
  <img width="12" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/dotnetcore/dotnetcore-original.svg" height="40" alt="dotnetcore logo"  />
  <img width="12" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/git/git-original.svg" height="40" alt="git logo"  />
  <img width="12" />
  <img src="https://cdn.jsdelivr.net/gh/devicons/devicon/icons/visualstudio/visualstudio-plain.svg" height="40" alt="visualstudio logo"  />
  <img width="12" />
</div>
