

![SQLite Database](image.png)

### Run and Test the MCP Server

- Using the MCP Inspector tool.
- Open the command prompt and locate to the project.
- Run the below command.
```bash
> npx @modelcontextprotocol/inspector dotnet run
```
This will start the MCP server with MCP Inspector.
![MCP Inspector start mcp server](image-1.png)

On the opened browser select connect.
Click Tools > List Tools to list the tools available in the MCP server.
![Tools list](image-2.png)

### Connect MCP through VS Code Copilot
- Copy the path of your csproj. 
- Create a new folder named .vscode
- Inside the folder create a file named mcp.json.
- Double click and open the mcp.json file.
- In VSCode you will get a Add Server button. 
- Click the Add server and select command(stdio)

![stdio](image-3.png)

![Enter command path](image-4.png)

![Name for the MCP server](image-5.png)

![alt text](image-6.png)

### Now ask the questions.
Question 1: Can  you explain what the BookCraft database looks like? Please list all the tables and how they are connected(relations between the tables). Group them in a way that makes sense.

- This will connect to our MCP server and ask for permission to run the get_tables_info.
![alt text](image-7.png)