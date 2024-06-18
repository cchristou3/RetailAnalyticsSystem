const PROXY_CONFIG = [
  {
    context: [
      "/_api",
      "/swagger",
    ],
    target: "http://localhost:5002",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
