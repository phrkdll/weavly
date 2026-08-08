# Weavly 🧶

**Weave your backend!**

_Modular API starter kit for modern applications._

Available via [NuGet](https://www.nuget.org/packages?q=Weavly&includeComputedFrameworks=false&prerel=true).

## 🚨 Caution

*The project is still at a very early stage. Some things may even be broken. Do not use in production!*

## 🚀 What is Weavly?

Weavly is a modular starter framework for ASP.NET Core projects.  
It helps you to quickly **assemble**, **connect**, and **scale** your backend systems using an **event-driven
architecture**.

Whether you’re building **microservices** or a **modular monolith** – Weavly’s plug-and-play modules have you covered.

## ✨ Key Features

- 🧩 Modular architecture
- ⚡ Event-driven module communication
- 🚀 Fast project bootstrap
- 🛠️ Easy microservice & monolith integration
- 🔄 Flexible and scalable

## 📦 Available Modules

- Weavly.Core
- Weavly.Auth
- Weavly.Mail
- Weavly.Logging
- Weavly.Configuration

*(More to come…)*

## 🛠️ Getting Started

```shell
dotnet nuget add source --name weavly https://codeberg.org/api/packages/phreakadelle/nuget/index.json

dotnet tool install -g weavly.cli
wv init
```