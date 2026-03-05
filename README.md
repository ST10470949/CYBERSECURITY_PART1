# Cyber Times With Abo  
### Programming 2A – PROG6221/w  
Console-Based Cybersecurity Awareness Chatbot  

##  Project Overview

Cyber Times With Abo is a console-based cybersecurity awareness chatbot developed in C#.  
The application educates users about common cyber threats and promotes safe online behaviour.

The system includes:

- Animated boot sequence
- Optional voice greeting
- ASCII startup branding
- Personalized user greeting with persistent tracking
- Menu-driven chatbot
- Free-form question handling
- Fuzzy matching for misspellings
- External extended-topic chatbot module
- Data persistence using file storage

## 🎯 Project Objectives

The main objectives of this project are:

- To demonstrate structured C# programming
- To implement user input validation
- To apply file handling for persistence
- To implement modular programming principles
- To integrate fuzzy matching using Levenshtein Distance
- To simulate a professional cybersecurity assistant

## 🧱 System Architecture

The application is structured into the following major components:

### 1️⃣ Startup System
- `BootSequence()` – Simulated system initialization
- `PlayVoiceGreeting()` – Plays optional WAV audio file
- `DisplayLogo()` – Displays ASCII branding

### 2️⃣ User Management
- `AskUserName()` – Input validation and persistence
- `LoadUserCounts()` – Reads stored visit counts
- `SaveUserCounts()` – Writes updated visit counts

User data is stored in:

user_counts.txt

### 3️⃣ Main Chat System
- `StartChat()` – Primary menu loop
- `DisplayMenu()` – User interface menu
- `TypeText()` – Typing animation effect
- `ShowError()` – Error handling messages

---

### 4️⃣ External Extended Chatbot
Implemented as a nested static class:


ExternalChatbot


Features:
- Separate extended topic menu
- Keyword recognition
- Multi-topic handling
- Fuzzy spelling detection
- Confirmation-based correction

### 5️⃣ Cybersecurity Topics Covered

- Password Security
- Phishing Awareness
- Malware
- Ransomware
- Public Wi-Fi Risks
- Safe Browsing
- Software Updates
- Identity Theft
- Social Engineering
- Two-Factor Authentication

## 🧠 Advanced Feature: Fuzzy Matching

The system uses a custom implementation of:


LevenshteinDistance()


This allows the chatbot to:
- Detect spelling mistakes
- Suggest possible corrections
- Ask the user to confirm suggested topics

Example:
If user types:
phising

System suggests:
Did you mean "phishing"?

## 🔐 Data Persistence
User visit tracking is implemented using file-based storage:
Format:

Name:VisitCount
If a returning user is detected:
Hey, welcome back [Name].

This demonstrates:
- File I/O
- Dictionary usage
- Exception handling
- State persistence
