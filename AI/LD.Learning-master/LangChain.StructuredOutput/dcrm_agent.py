import uuid
import requests
import os
from dotenv import load_dotenv
import json

# Load environment variables from .env file
load_dotenv(dotenv_path='appsettings.env')

function_descriptions = [
    {
        "name": "get_conversation_sentiment",
        "description": "Get conversation analysis for a given conversation ID.",
        "parameters": {
            "type": "object",
            "properties": {
                "conversation_id": {
                    "type": "string",
                    "description": "The ID of the conversation."
                }
            },
            "required": ["conversation_id"]
        }
    },
    {
        "name": "get_customer_service_information_by_email",
        "description": "Get customer service information for a given email.",
        "parameters": {
            "type": "object",
            "properties": {
                "email": {
                    "type": "string",
                    "description": "The email of a lead."
                }
            },
            "required": ["email"]
        }
    },
      {
        "name": "get_loan_information_by_empowerloanId",
        "description": "Get loan information for a given empower loan id.",
        "parameters": {
            "type": "object",
            "properties": {
                "empowerloanId": {
                    "type": "string",
                    "description": "The empowerloanId of a lead."
                }
            },
            "required": ["empowerloanId"]
        }
    }
]

import requests
import json

def get_token(client_id, client_secret, tenant_id, scope):
    token_url = f"https://login.microsoftonline.com/{tenant_id}/oauth2/v2.0/token"
    request_data = {
        "client_id": client_id,
        "client_secret": client_secret,
        "scope": scope,
        "grant_type": "client_credentials"
    }
    response = requests.post(token_url, data=request_data)
    
    if response.status_code == 200:
        response_data = response.json()
        if "access_token" in response_data:
            return response_data["access_token"]
        else:
            raise Exception("Access token not found in response.")
    else:
        raise Exception(f"Failed to retrieve token: {response.status_code} - {response.text}")

def get_customer_service_information_by_email(email: str):
    client_id = "77d2e34e-9638-4fa5-97d5-4007736b127e"
    client_secret = "dgi8Q~uXana7f.PK2.Y_14PTeWchez09EjYSccIR"
    tenant_id = "19479f88-8eac-45d2-a1bf-69d33854a3fa"
    scope = "https://d365-customerservice-qa1.crm.dynamics.com/.default"

    # Get the access token
    token = get_token(client_id, client_secret, tenant_id, scope)

    # Set the headers for the request
    headers = {
        "Authorization": f"Bearer {token}",
        "Accept": "application/json"
    }

    # Prepare the request URL
    base_url = "https://d365-customerservice-qa1.crm.dynamics.com/api/data/v9.2/"
    query = "contacts?$select=fullname&$filter=emailaddress1 eq '{0}'"    
    request_url = query.format(email)

    print(f"get_customer_service_information_by_email: {base_url}")

    # Send the GET request
    response = requests.get(base_url + request_url, headers=headers)

    if response.status_code == 200:
        json_response = response.json()

        # Deserialize the response to retrieve the origination case
        origination_case = json_response.get("OriginationCase")

        return origination_case
    else:
        raise Exception(f"API request failed: {response.status_code} - {response.text}")
    
def get_loan_information_by_empowerloanId(empowerloanId: str):

    # Prepare the request URL
  
    headers = {'Content-Type':'application/json',
               'Ocp-Apim-Subscription-Key':'d5ea52df36484441bf4e718c3d58d9f8',
               'Authorization':'ApiKey Key=AdxjOEvHDJ0QEFbQLyv5yULioAviYAOtXS36sVmYpuf7msfleotowVAqqQOEGc7pUFGIwUNtQnupkPWhZO6xX5QdK1vsw5u59WBl4Sg53Cr01IvkXpgI-ba6CSMJuaTyXEJzqxiKRDt04-nHNGZwADj916QtFecL7GU00fKkvXd_9X5357alvr4w560eigr1l6rRtLwGGCDOh3V5sq-cLr-sTqkoRKRy0rkbWD0z0L2uPJsN0xzE0kKn53MXcXbEKA2',
               'LendingSessionId':'f542ab7c-4016-4508-8ee3-9165d25f9ef4',
               'LendingUserName':'epsimrserviceuser'}
        
    try:
         base_url = "http://ldapi-dv1.loandepotdev.works"
         print(f"get_loan_information_by_empowerloanId: {base_url}")
         
         response = requests.get(
                f"{base_url}/EPS-Lending/api/v1/lending/mortgage/loans/{empowerloanId}?lPart=All&bPart=All",                 
                headers=headers
            )

         if response.status_code == 200:
                ePS_loans_response = response.json()
                return ePS_loans_response
    except Exception as ex:
            print(f"Error: {ex}")

    return None

def get_conversation_sentiment(conversation_id: str):
    # Conversation API endpoint and API key
    conversationapi_endpoint = os.getenv("ENTERPRISE_CONVERSATIONSERVICE_BASEURL")
    print(f"conversationapi: {conversationapi_endpoint}")

    # Define headers
    headers = {
        "Content-Type": "application/json"        
    }

    # Make the API call
    response = requests.get(f"{conversationapi_endpoint}/analysis/Conversation/{conversation_id}",  headers=headers)
    
    # Handle response
    if response.status_code == 200:
        return response.json()
    else:
        return {"error": f"{response.text}  {conversationapi_endpoint}"}  # Corrected error string formatting

def ask_llm(query: str):
    # Azure OpenAI endpoint and API key
    azure_endpoint = os.getenv("AZURE_OPENAI_ENDPOINT")
    api_key = os.getenv("AZURE_OPENAI_API_KEY")
    print(f"Azure Endpoint: {azure_endpoint}")
    print(f"API Key: {api_key}")  

    # Define the messages for the request
    messages = [
        {
            "role": "system",
            "content": "You are an expert in answering conversation and loan information questions"
        },
        {
            "role": "user",
            "content": f"{query}."
        }
    ]    

    # Define the payload for the Azure OpenAI API
    payload = {
        "model": "gpt-4",  # Example model
        "messages": messages,
        "functions": function_descriptions,
        "function_call": "auto"
    }

    # Define headers
    headers = {
        "Content-Type": "application/json",
        "Ocp-Apim-Subscription-Key": f"{api_key}"
    }

    # Make the API call
    response = requests.post(f"{azure_endpoint}/LLM-POC/openai/deployments/LD-Chat/chat/completions?api-version=2024-02-15-preview", json=payload, headers=headers)
    
    result = response.json()

    # Handle response
    if response.status_code == 200:
        if 'choices' in result and 'message' in result['choices'][0]:
            function_calls = result['choices'][0]['message'].get('function_call', None)
            if function_calls:
                # Handle one or more function calls
                responses = []
                # Make sure function_calls is iterable, handle single or multiple function calls
                if isinstance(function_calls, list):  # If multiple function calls
                    for function_call in function_calls:
                        responses.append(handle_function_call(function_call))
                else:  # Handle single function call
                    responses.append(handle_function_call(function_calls))
                
                return responses  # Return list of results from multiple function calls
            else:
                return result['choices'][0]['message']['content']
        else:
            return result['choices'][0]['message']['content']
    else:
        return {"error": response.text}

def handle_function_call(function_call):
    # Extract arguments and function name from the function call
    arguments_str = function_call['arguments']
    arguments = json.loads(arguments_str)
    chosen_function_name = function_call['name']

    # Ensure chosen_function is a callable function
    chosen_function = globals().get(chosen_function_name)
    if callable(chosen_function):
        # Call the function with the arguments
        return chosen_function(**arguments)
    else:
        raise TypeError(f"{chosen_function_name} is not a callable function")
