import pyodbc


class SQLClient:

    def __init__(self):

        # Define the connection string
        self.connection_string = (
            "DRIVER={ODBC Driver 17 for SQL Server};"
            "SERVER=your_server_name;"
            "DATABASE=your_database_name;"
            "UID=your_username;"
            "PWD=your_password"
        )

        # Establish a connection to the database
        conn = pyodbc.connect(self.connection_string)
        cursor = conn.cursor()

        # Define the query
        query = "SELECT * FROM your_table_name"

        # Execute the query
        cursor.execute(query)

        # Fetch all rows from the executed query
        rows = cursor.fetchall()

        # Print the column names
        columns = [column[0] for column in cursor.description]
        print(columns)

        # Print each row
        for row in rows:
            print(row)

        # Close the connection
        cursor.close()
        conn.close()



import pyodbc

# Get the list of ODBC drivers installed on the system
drivers = pyodbc.drivers()

# Define the connection string
connection_string = (
    "DRIVER={ODBC Driver 18 for SQL Server};"
    "SERVER=PAMBOS\\SQLEXPRESS;"
    "DATABASE=InspireDatabase;"
    "UID=sa;"
    "PWD=Password1!;"
    "TrustServerCertificate=yes"
)

# Establish a connection to the database
conn = pyodbc.connect(connection_string)
cursor = conn.cursor()

# Define the query
query = "SELECT DISTINCT [Tags] FROM SalesDataOnlineShop"

# Execute the query
cursor.execute(query)

# Fetch all rows from the executed query
rows = cursor.fetchall()

# Print the column names
columns = [column[0] for column in cursor.description]
print(columns)

tags = ''
for row in rows:
    '["Radio Control", "Realistic Sound"]'
    value: str = row[0]
    tags += value.replace('[', '').replace(']', '').replace('"', '') + ','

tag_list = set(tags.split(','))

for tag in tag_list:
    print(f"('{tag}'),")

print(tags)
print()
print(tag_list)

# Close the connection
cursor.close()
conn.close()

# Print the list of drivers
print("ODBC Drivers Installed:")
for driver in drivers:
    print(driver)
