# Pharmacy Management System - Desktop Application (ASP.NET C#)

A modern and efficient **Pharmacy Management Desktop Application** built using **ASP.NET with C#** and **Microsoft SQL Server**. The system is designed for both **pharmacy owners (Admins)** and **employees**, providing an intuitive interface for managing customers, medicines, inventory, billing, and user accounts with login tracking.

---

## 🧾 Features

### 🔐 Authentication & Role Management
- **Secure Login System**
- **Role-based Access**: Separate dashboards for `Admin` and `Employee`
- **Login Activity Tracking**
- **Admin can add and manage employees**

### 🧍‍♂️ Customer Management
- Add new customers
- Update existing customer details
- Delete customer records
- Search customers by name or phone number

### 👥 Employee Management (Admin Only)
- Add new employees with role and credentials
- Edit employee details
- Delete employees from the system
- View login activity of all users

### 💊 Medicine & Inventory Management

- Manage inventory stock (quantity, batch number, invoice, tracking number)
- Filter inventory by `Inventory ID` or `Medicine ID`

### 💵 Billing System
- Create new bills for customers
- Add multiple medicines under a single Sale ID
- Auto-calculate total price per sale
- Record bills under the employee or admin who made them

### 📊 Dashboards
- **Admin Dashboard**:
  - Full access to all features (Customers, Inventory, Medicine, Sales, Employees)
  - View and manage all employee accounts
  - View all sales made by any user
- **Employee Dashboard**:
  - Restricted access (Sales and Add User feature not available.)
  - Track personal sales only

### 📅 Sorting & Filtering
- Sort sales by date (Recent to Old, Old to Recent)
- Filter sales using DateTimePicker

---

## 🧰 Tech Stack

| Layer         | Technology                  |
|---------------|-----------------------------|
| Language      | C# (.NET Framework)         |
| UI Framework  | Windows Forms               |
| Backend       | ASP.NET                     |
| Database      | Microsoft SQL Server        |
| ORM / Access  | ADO.NET / SQL Commands      |
| Auth & Roles  | Custom role-based login     |

---

## 📂 Project Structure

```plaintext
Pharmacy_Management/
│
├── Forms/
│   ├── Login.cs
├── AdminDashboard.cs
│   ├── EmployeeDashboard.cs
│   ├── Inventory.cs
│   ├── Customer.cs
│   ├── Billing.cs
│   └── ManageEmployees.cs
│
├── Database/
│   └── Pharmacy_Management.sql (schema & seed data)
│
├── Models/
│   └── User.cs, Medicine.cs, Sale.cs, etc.
│
├── App.config (connection string)
├── Program.cs
└── README.md
