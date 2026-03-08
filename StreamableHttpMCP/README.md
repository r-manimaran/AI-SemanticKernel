

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