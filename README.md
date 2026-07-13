# EcoFleet Logistik 🚛

> **Projekt-Übersicht:** Eine moderne, cloudnative SaaS-Plattform für intelligentes Logistikmanagement, entwickelt mit .NET 9 und Azure.
> *(For the full technical documentation, implementation details, and architecture diagrams, please see the English section below.)*

---

## 🌐 Overview
**eco-fleet-logistics** is an enterprise-grade, cloud-native logistics management platform designed to optimize fleet operations, track shipments in real-time, and streamline supply chain communication. 

This repository serves as a production-ready showcase of modern software engineering practices, featuring **Clean Architecture**, **Microservices design patterns**, and robust **DevOps automation (CI/CD)** within the .NET 9 ecosystem.

## 🏗️ Architectural Core
The system is built using **Clean Architecture** (Onion Architecture) principles to ensure high maintainability, testability, and strict decoupling of core business logic from external infrastructures and frameworks.

*   **Domain:** Core enterprise business rules, aggregates, entities, and value objects (Pure C#, zero external dependencies).
*   **Application:** CQRS pattern orchestration using MediatR, use cases, fluid validations, and custom exceptions.
*   **Infrastructure:** EF Core data access layer (PostgreSQL), data migrations, event logging, and external communication adapters.
*   **API (Presentation):** ASP.NET Core Web API acting as the secure entry point for external clients.

## 🚀 Architectural Evolution
To mirror real-world enterprise scaling, the system is designed to evolve organically:
1.  **Monolithic Core:** Establishing a rock-solid domain model and data persistence layer.
2.  **Distributed Microservices:** Breaking down core logic into autonomous services communicating asynchronously via **RabbitMQ** and **MassTransit** using the **Outbox Pattern**.
3.  **Cloud-Native & DevOps:** Full containerization (**Docker**), automated pipelines (**GitHub Actions**), and **Infrastructure as Code (IaC)** deployment to Microsoft Azure.

---
*For real-time project management, task breakdowns, and current sprint backlogs, please refer to the dedicated **GitHub Projects** tab of this repository.*
