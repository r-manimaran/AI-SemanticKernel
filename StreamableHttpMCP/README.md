# MCP Server with Streamable Http 
![alt text](image-13.png)

![alt text](image.png)

- Test using MCP Inspector

![alt text](image-2.png)

![alt text](image-1.png)

## Connect to Claude Desktop.
- Install the Claude Desktop.
- Go to settings --> Developer and update the claude config file.
- Add the below entry.

```json
{
   "mcpServers": {
    "my-weather-server": {
      "command": "mcp-remote",
      "args": [
        "http://localhost:5000/mcp"
      ]
    }
  },
  "preferences": {
    "coworkWebSearchEnabled": true,
    "coworkScheduledTasksEnabled": false,
    "ccdScheduledTasksEnabled": false,
    "sidebarMode": "chat"
  }
}
```

- Restart the Cluade Desktop. Ensure your MCP server application is running in the background.
- In the settings 

![alt text](image-3.png)

![alt text](image-4.png)

![alt text](image-5.png)

## Docker Desktop Tools

![alt text](image-9.png)

![alt text](image-6.png)

![alt text](image-7.png)

![alt text](image-8.png)


Here are some examples

"Give me a summary of all my Docker containers"
"Show me logs from my postgres container"
"Which containers are currently stopped?"
"Get CPU and memory usage for my api container"
"Show details of the redis container including its mounts"

## Consume in Microsoft Agent Framework 

![alt text](image-10.png)

![alt text](image-11.png)

![alt text](image-12.png)

![alt text](image-14.png)

![alt text](image-15.png)

![alt text](image-16.png)