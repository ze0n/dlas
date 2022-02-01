import dash
from dash import html
import dash_bootstrap_components as dbc
from dash.dependencies import Input, Output
import pandas as pd
from dash import dcc
from pymongo import MongoClient
import plotly.express as px

class SessionsDataRepository:

    def __init__(self):
        self.client = MongoClient('mongodb://localhost:27017/')
        self.db = self.client.dlas

    def get_sessions(self):
        return self.db.list_collection_names()

    def get_session_elements(self, sessionId):
        raw = list(self.db[sessionId].find({},{"ValueInfo._id": 1}))
        return set(map(lambda x: x["ValueInfo"]["_id"], raw))

    def get_session_element(self, sessionId, var):
        raw = list(self.db[sessionId].find({"ValueInfo._id": var}))
        return raw

    def clean_sessions(self):
        for session in self.get_sessions():
            self.db[session].drop()


REPO = SessionsDataRepository()

app = dash.Dash(
    external_stylesheets=[dbc.themes.BOOTSTRAP],
    suppress_callback_exceptions=True
)

app.layout = html.Div(
    [
        dcc.Location(id='url', refresh=False),
        dcc.Interval(
            id='update-interval',
            interval=1*1000, # in milliseconds
            n_intervals=0
        ),
        dbc.Row(
            dbc.Col(
                html.H2("Debug Like A Scientist"),
                width={"size": 9, "offset": 0},
            ),
        ),
        dbc.Row(
            dbc.Col(
                dbc.Breadcrumb(
                    id="breadcrumbs"
                ),
                width={"size": 12, "offset": 0},
            )
        ),
        dbc.Row(id='page-content')
    ],
    style={
        "padding": "20px"
    }
)

def render_session(sessionId):
    return [
            dbc.Col(
                html.Div([
                    dbc.Row([
                        dbc.Col(
                            html.H3("Scope"),
                            width={"size": 12},
                        ),
                    ]),
                    dbc.Row([
                        dbc.Col(
                            [
                                html.Div(id='var-list'),
                            ],
                            width={"size": 12},
                        ),
                    ])
                ]),
                width={"size": 3, "order": "first", "offset": 0},
                style={
                    "background-color": "#EEE"
                }
            ),
            dbc.Col(
                html.Div([
                    dbc.Row([
                        dbc.Col(
                            html.H3("Visualization"),
                            width={"size": 3},
                        ),
                    ]),

                    dbc.Row([
                        dbc.Col([
                            html.Span("Type"),
                            ],
                            width={"size": 1},
                        ),
                        dbc.Col([
                            dcc.Dropdown(
                                id='visualization-type-dropdown',
                                options=[
                                    {'label': 'Histogram', 'value': 'Histogram'},
                                    {'label': 'Timeseries', 'value': 'Timeseries'},
                                    {'label': 'Scatterplot vs time', 'value': 'Scatterplot_vs_time'}
                                ],
                                value='Histogram'
                            )],
                            width={"size": 4},
                        ),
                    ]),

                    dbc.Row([
                        dbc.Col(
                            [
                                dcc.Graph(id='visualization'),
                            ],
                            width={"size": 12},
                        ),
                    ])
                ]),
                width={"size": 9, "order": 1, "offset": 0},
            ),
        ]



def render_session_list():
    return [dbc.Col([
        dbc.Row([
            dbc.Col(
                html.H3("Sessions"),
                width={"size": 12},
            ),
        ]),
        dbc.Row([
            dbc.Col(
                [
                    html.Div(id='sessions-list'),
                ],
                width={"size": 12},
            ),
        ]),
        dbc.Button("Clean", color="danger", className="me-1", id="btn-clean-sessions", n_clicks=0),
        html.Div(id="hidden-div", style = {'display':'none'})
    ])]


@app.callback(
    Output("hidden-div", "children"),
    [Input("btn-clean-sessions", "n_clicks")]
)
def on_button_click(n):
    if n is None or n == 0:
        pass
    else:
        REPO.clean_sessions()
    return []

@app.callback(
    Output(component_id='sessions-list', component_property='children'),
    [dash.dependencies.Input('url', 'pathname'),
     Input('update-interval', 'n_intervals')])
def update_sessions_list(input_value, n_intervals):
    sessions = REPO.get_sessions()
    v = list(map(lambda x: dbc.NavItem(dbc.NavLink(f"{x}", active=True, href=f"/sessions/{x}")), sessions))
    return v

@app.callback(dash.dependencies.Output('breadcrumbs', 'items'),
              [dash.dependencies.Input('url', 'pathname')
               ])
def update_breadcrumbs(pathname):
    elements = list(filter(lambda x: x != "", pathname.split("/")))
    brs = ["/"]
    for i in range(len(elements)):
        brs.append({"label": elements[i], "href": "/" + "/".join(elements[:i+1]), "external_link": False})
    return brs

@app.callback(dash.dependencies.Output('page-content', 'children'),
              [dash.dependencies.Input('url', 'pathname')])
def display_page(pathname):

    if(pathname in ["/sessions", "/sessions/"]):
        return render_session_list()

    elements = pathname.split("/")

    if(len(elements) > 1 and elements[1] == "sessions"):
        print(elements)
        return render_session(elements[2])
    else:
        return render_session_list()

@app.callback(
    Output(component_id='var-list', component_property='children'),
    [dash.dependencies.Input('url', 'pathname')]
)
def update_var_list(pathname):

    elements = list(filter(lambda x: x!= "", pathname.split("/")))

    print("yo", elements)

    if(len(elements)<2):
        return []

    sessionId = elements[1]

    vars = REPO.get_session_elements(sessionId)
    v = list(map(lambda x: dbc.NavItem(dbc.NavLink(f"{x} (Scalar, 35)", active=True, href=f"/sessions/{sessionId}/{x}")), vars))
    return v

@app.callback(dash.dependencies.Output('visualization', 'figure'),
              [
                dash.dependencies.Input('url', 'pathname'),
                Input('update-interval', 'n_intervals'),
                Input(component_id='visualization-type-dropdown', component_property='value')
              ])
def update_visualization(pathname, n_intervals, vis_type):
    elements = list(filter(lambda x: x != "", pathname.split("/")))

    if(len(elements)<3):
        return {}

    sessionId = elements[1]

    var = elements[2]
    el = REPO.get_session_element(sessionId, var)

    def selector(x):
        return {
            "name": x["ValueInfo"]["_id"],
            "value": x["ValueInfo"]["Value"],
            "timestamp": x["Context"]["Timestamp"],
        }

    records = list(map(selector, el))

    df = pd.DataFrame(records)

    if(vis_type == "Histogram"):
        return {
                'data': [
                    {
                        'x': df['value'],
                        'text': df['name'],
                        #'customdata': df['storenum'],
                        'name': 'Open Date',
                        'type': 'histogram'
                    }
                ],
                'layout': {
                    "bargap": 0.2
                }
            }
    elif(vis_type == "Timeseries"):
        return {
                'data': [
                    {
                        'x': df['timestamp'],
                        'y': df['value'],
                        'text': df['name'],
                        #'customdata': df['storenum'],
                        'name': 'Open Date',
                        'mode': 'lines+markers',
                        'type': 'scatter'
                    }
                ],
                'layout': {
                    "bargap": 0.2
                }
            }
    elif(vis_type == "Scatterplot_vs_time"):
        return {
                'data': [
                    {
                        'x': df['timestamp'],
                        'y': df['value'],
                        'text': df['name'],
                        #'customdata': df['storenum'],
                        'name': 'Open Date',
                        "mode": "markers",
                        'type': 'scatter'
                    }
                ],
                'layout': {
                    "bargap": 0.2
                }
            }

if __name__ == "__main__":
    app.run_server(debug=True)