import json

def hello(p):
  print("--- In script ---")
  print("First: " + p["First"])
  print("Last: " + p["Last"])
  print("Age: " + str(p["Age"]))
  resp = {}
  resp["First"] = "Lucas"
  resp["Last"] = "Christner"
  resp["Age"] = 4
  return json.dumps(resp)
