import pandas as pd
from sqlalchemy import create_engine, MetaData, Table, update, Connection, Engine
from sqlalchemy.orm import sessionmaker


class SQLClient:
    def __init__(self):
        self.engine: Engine = create_engine(
            "mssql+pyodbc://sa:Password1!@PAMBOS\\SQLEXPRESS/InspireDatabase?driver=ODBC+Driver+18+for+SQL+Server&TrustServerCertificate=yes")
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

    def get_top_revenue_generating_cities(self) -> pd.DataFrame:
        query = """
            ;WITH CTE_AGGREGATE_INVOICES_BY_CITY
            AS
            (
                SELECT 
                    CityName = CI.[Name],
                    Value = SUM(ID.Quantity * ID.UnitPrice),
                    Volume = SUM(ID.Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                INNER JOIN Customers C			ON I.CustomerId = C.Id
                INNER JOIN Cities CI			ON CI.Id = C.CityId
                GROUP BY CI.[Name]
            )
            SELECT TOP 10
                CTE.CityName,
                CTE.Value,
                CTE.Volume
            FROM CTE_AGGREGATE_INVOICES_BY_CITY CTE
            ORDER BY CTE.Value DESC
           """
        return self.execute_select(query)

    def get_top_revenue_generating_products(self) -> pd.DataFrame:
        query = """
            ;WITH CTE_AGGREGATE_INVOICES_BY_PRODUCT
            AS
            (
                SELECT 
                    ProductName = P.[Name],
                    Value = SUM(ID.Quantity * ID.UnitPrice),
                    Volume = SUM(ID.Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                INNER JOIN Products P			ON ID.ProductId = P.Id
                GROUP BY P.[Name]
            )
            SELECT TOP 5
                CTE.ProductName,
                CTE.Value,
                CTE.Volume
            FROM CTE_AGGREGATE_INVOICES_BY_PRODUCT CTE
            ORDER BY CTE.Value DESC
           """
        return self.execute_select(query)

    def get_sales_by_customer_category(self) -> pd.DataFrame:
        query = """
            ;WITH CTE_AGGREGATE_INVOICES_BY_CUSTOMER_CATEGORY
            AS
            (
                SELECT 
                    CustomerCategoryName = CI.[Name],
                    Value = SUM(ID.Quantity * ID.UnitPrice),
                    Volume = SUM(ID.Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
                INNER JOIN Customers C				ON I.CustomerId = C.Id
                INNER JOIN CustomerCategories CI	ON CI.Id = C.CustomerCategoryId
                GROUP BY CI.[Name]
            )
            SELECT
                CTE.CustomerCategoryName,
                CTE.Value,
                CTE.Volume
            FROM CTE_AGGREGATE_INVOICES_BY_CUSTOMER_CATEGORY CTE
            ORDER BY CTE.Value DESC
           """
        return self.execute_select(query)

    def get_customer_distribution_by_customer_category(self) -> pd.DataFrame:
        query = """
             SELECT 
                CustomerCategoryName = C.[Name], 
                NumberOfCustomers = COUNT(1)
            FROM CustomerCategories C
            INNER JOIN Customers CU			ON C.Id = CU.CustomerCategoryId
            GROUP BY C.Name
            ORDER BY COUNT(1) DESC
           """
        return self.execute_select(query)

    def get_customer_distribution_by_customer_segment(self) -> pd.DataFrame:
        query = """
             SELECT 
                    SegmentName = Segment, 
                    NumberOfCustomers = COUNT(1) 
             FROM Customers
             GROUP BY Segment
             ORDER BY COUNT(1) DESC
           """
        return self.execute_select(query)

    def get_sales_by_product_pack_type(self) -> pd.DataFrame:
        query = """
            ;WITH CTE_AGGREGATE_INVOICES_BY_PRODUCT_PACK_TYPE
            AS
            (
                SELECT 
                    ProductPackTypeName = PPT.[Name],
                    Value = SUM(ID.Quantity * ID.UnitPrice),
                    Volume = SUM(ID.Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID			ON I.Id = ID.InvoiceId
                INNER JOIN Products P				ON ID.ProductId = P.Id
                INNER JOIN ProductPackageTypes PPT	ON PPT.Id = P.PackageTypeId
                GROUP BY PPT.[Name]
            )
            SELECT 
                CTE.ProductPackTypeName,
                CTE.Value,
                CTE.Volume
            FROM CTE_AGGREGATE_INVOICES_BY_PRODUCT_PACK_TYPE CTE
            ORDER BY CTE.Value DESC
           """
        return self.execute_select(query)

    def get_sales_by_product_tag(self) -> pd.DataFrame:
        query = """
            ;WITH CTE_PRODUCT_TAGS
            AS
            (
                SELECT 
                    PT.ProductId, 
                    Tags = T.Name
                FROM ProductTags PT
                INNER JOIN Tags T				ON T.Id = PT.TagId
            ),
            CTE_AGGREGATE_INVOICES
            AS
            (
                SELECT 
                    PT.Tags,
                    Value = SUM(ID.Quantity * ID.UnitPrice),
                    Volume = SUM(ID.Quantity)
                FROM Invoices I
                INNER JOIN InvoiceDetail ID		ON I.Id = ID.InvoiceId
                INNER JOIN Products P			ON P.Id = ID.ProductId
                LEFT JOIN CTE_PRODUCT_TAGS PT		ON P.Id = PT.ProductId
                GROUP BY PT.Tags
            )
            SELECT
                ProductTagName = ISNULL(CTE.Tags, 'Untagged'),
                CTE.[Value],
                CTE.Volume
            FROM CTE_AGGREGATE_INVOICES CTE
            ORDER BY ISNULL(CTE.Tags, 'Untagged')
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
