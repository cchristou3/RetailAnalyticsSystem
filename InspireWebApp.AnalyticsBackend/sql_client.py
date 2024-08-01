import pandas as pd
from sqlalchemy import create_engine, MetaData, Table, update, Connection, Engine
from sqlalchemy.orm import sessionmaker


class SQLClient:
    def __init__(self):
        self.engine: Engine = create_engine("mssql+pyodbc://sa:Password1!@PAMBOS\\SQLEXPRESS/InspireDatabase?driver=ODBC+Driver+18+for+SQL+Server&TrustServerCertificate=yes")
        self.connection: Connection = self.engine.connect()
        self.metadata: MetaData = MetaData()
        self.Session = sessionmaker(bind=self.engine)

    def execute_select(self, query: str) -> pd.DataFrame:
        with self.connection.begin():
            return pd.read_sql(query, self.connection)

    def get_rfm_prerequisite_data(self) -> pd.DataFrame:
        query = """
           SELECT
               [CustomerId]	= CustomerId,
               [InvoiceId]		= I.Id,
               [InvoiceDate]	= CAST(I.Date AS DATE),
               [TotalPrice]	= ID.Quantity * ID.UnitPrice
           FROM Invoices I
           INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
           """
        return self.execute_select(query)

    def get_fpm_prerequisite_data(self) -> pd.DataFrame:
        query = """
           SELECT
                [CustomerId]		= CustomerId,
                [InvoiceId]			= I.Id,
                [InvoiceDate]		= CAST(I.[Date] AS DATE),
                [TotalPrice]		= ID.Quantity * ID.UnitPrice,
                [CustomerSegment]	= CU.Segment,
                [ProductName]		= P.[Name],
                [CustomerRfmScore]  = CU.RfmScore
            FROM Invoices I
            INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
            INNER JOIN Customers CU				ON CU.Id = I.CustomerId
            INNER JOIN Products P				ON P.Id = ID.ProductId
           """
        return self.execute_select(query)

    def get_describe_prerequisite_data(self) -> pd.DataFrame:
        query = """
            SELECT
                [CustomerId]		= CustomerId,
                [InvoiceId]			= I.Id,
                [InvoiceDate]		= I.[Date],
                [UnitPrice]		    = ID.UnitPrice,
                [Quantity]          = ID.Quantity,
                [CustomerSegment]	= CU.Segment,
                [ProductName]		= P.[Name],
                [CustomerRfmScore]  = CU.RfmScore,
                [PackageType]		= PPT.[Name],
                [CustomerCategory]	= CC.[Name],
                [CityName]			= CI.[Name]
            FROM Invoices I
            INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
            INNER JOIN Customers CU				ON CU.Id = I.CustomerId
            INNER JOIN Products P				ON P.Id = ID.ProductId
            INNER JOIN ProductPackageTypes PPT	ON PPT.Id = P.PackageTypeId
            INNER JOIN CustomerCategories CC	ON CC.Id = CU.CustomerCategoryId
            INNER JOIN Cities CI				ON CI.Id = CU.CityId
           """
        return self.execute_select(query)

    def get_arima_prerequisite_data(self) -> pd.DataFrame:
        query = """
            SELECT
                [InvoiceDate]		= CAST(I.[Date] AS DATE),
                [TotalSales]		= ID.Quantity * ID.UnitPrice,
                [CustomerSegment]	= CU.Segment
            FROM Invoices I
            INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
            INNER JOIN Customers CU				ON CU.Id = I.CustomerId
           """
        return self.execute_select(query)

    def update_customers(self, updates: list[dict]) -> None:
        with self.connection.begin():
            customers_table = Table('Customers', self.metadata, autoload_with=self.engine)
            for update_data in updates:
                stmt = (
                    update(customers_table)
                    .where(customers_table.c.Id == update_data['CustomerId'])
                    .values(RfmScore=update_data['RfmScore'], Segment=update_data['Segment'])
                )
                self.connection.execute(stmt)

    def insert_rules(self, rules: list[dict]) -> None:
        rules_table = Table('MinerAssocRules', self.metadata, autoload_with=self.engine)
        with self.connection.begin():
            try:
                self.connection.execute(rules_table.delete())
                self.connection.execute(rules_table.insert(), rules)
            except Exception as e:
                raise
