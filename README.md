# Class-based fighter game

## Description

This is a 2D Unity game that combines the combat of fighter platformers with the skill tree and customizable loadout of an RPG.

## Important links

- [Gitlab](https://git.las.iastate.edu/SeniorDesignComS/2022fall/class-based-fighter-game)
- [Design Documents](https://drive.google.com/drive/folders/1X5-LN5zrB9eLN_YNaz4BDINnO8pxLjnl?usp=sharing)

## Setup

To run the project, you will need to build the client application, build the server application, and set up the API.

### Building the game

- Open the Class-Based-Fighting-Directory in the Unity Hub.
  See Unity's [official site](https://unity.com/download) for information on installing the Hub.
  _Note: This project currently requires Unity version 2021.3.9f1._

- Build and run the game (File > Build and run).
  This creates an executable at Class-Based-Fighting-Game/Builds/CBFG_0.0.0/

### Running the server

- Ensure [.NET](https://git.las.iastate.edu/SeniorDesignComS/2022fall/class-based-fighter-game/-/blob/readme/README.html) is installed on your machine.

- To run the game server locally, execute `dotnet run` from the server directory.
  This spins up a server on port 585.
  _Note: this requires dotnet version >= 6.0._

### Backend setup

Your machine will need

- PHP version >= 8
- [Composer](https://getcomposer.org/)
- An SQL server (for example, [MySQL](https://dev.mysql.com/downloads/mysql/))

#### Setting up the database

The project comes with built-in support for MySQL, SQLite, and Microsoft SQL Server.
The examples show here use MySQL.
For information on other databases, see the [Laravel documentation](https://laravel.com/docs/9.x/database#configuration).

Create the database and credentials the API will use. For example, using a MySQL database:

```sql
CREATE DATABASE classbasedfightergamedb;
CREATE USER classbasedfightergameuser IDENTIFIED BY 'classbasedfightergamepw';
GRANT ALL PRIVILEGES ON classbasedfightergamedb.* to 'classbasedfightergameuser'@'%';
```

#### Configuring the application

From the api directory, copy .env.example to a new file .env.
Replace the database credentials in .env with your credentials. For example:

```bash
DB_CONNECTION=mysql
DB_HOST=127.0.0.1
DB_PORT=3306
DB_DATABASE=classbasedfightergamedb
DB_USERNAME=classbasedfightergameuser
DB_PASSWORD=classbasedfightergamepw
```

For more information on configuring the application, see [Laravel documentation](https://laravel.com/docs/9.x/configuration)

#### Final steps

- Install project dependencies: `composer install`
- Create project tables: `php artisan migrate`
- Seed the database with built-in game information: `php artisan db:seed`
- Run the project locally: `php artisan serve`
  - This will spin up a server on port 8000. The port number can be changed in the .env file.
#### Contributors

- Yden Da
- Noah Cantrell
- Joseph Hoffmann
- Ryan Banks
- Matthew Renze
