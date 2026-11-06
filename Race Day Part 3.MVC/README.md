# 🏁 RaceDay Part 3 MVC Web Application

This is the front-end user interface system for the RaceDay Network. It communicates entirely as an upstream consumer of the Part 2 API network using server-side session mappings over HTTP channels.

## 🐳 Single-Command Docker Deployment
To compile and instantiate the fully containerized web front-end inside an isolated execution environment, execute the following block inside your system terminal console:

```bash
docker build -t raceday-frontend -f Dockerfile . && docker run -d -p 8080:8080 --name raceday-mvc-app raceday-frontend
```

Once initialized, navigate your computer's web browser to `http://localhost:8080` to evaluate the portal pages.

## 📚 Academic Integrity Citation
* **Generative Tool Utilized:** Google Gemini (Large Language Model)
* **Formal Bibliographical Entry:** Google (2026) *Gemini* (Large language model). Available at: https://google.com [Accessed: 2 November 2026].
