# Endpoint Plan

This document outlines the API endpoints required to interact with the RaceDay Database.

## Endpoints

* **GET /api/events**
  * **Description:** Retrieves a list of all scheduled race events.
  * **Response:** 200 OK with a list of event objects.

* **POST /api/participants**
  * **Description:** Registers a new participant into the system.
  * **Response:** 201 Created with the new participant details.

* **GET /api/enrolments**
  * **Description:** Retrieves all active race entries and registrations.
  * **Response:** 200 OK with registration details.
