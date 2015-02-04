import ConfigParser
import requests
from os.path import join

host = 'localhost'
service_name = 'UDSH/SurveySync'

configuration = 'Debug'
configuration = 'Stage'
# configuration = 'Release'

credentials = 'ArcGIS Admin Credentials'
destination = 'Destination'

config = ConfigParser.RawConfigParser()
file = join('./', 'secrets.cfg')
config.read(file)

host = config.get(destination, configuration)

print 'Uploading to {}'.format(host)

token_url = 'http://{}:6080/arcgis/admin/generateToken'.format(host)
update_soe_url = 'http://{}:6080/arcgis/admin/services/types/extensions/update'.format(
    host)
upload_url = 'http://{}:6080/arcgis/admin/uploads/upload?token={}'.format(
    host, '{}')
start_service_url = 'http://{}:6080/arcgis/admin/services/{}.MapServer/start'.format(
    host, service_name)

file_name = 'C:/Projects/GitHub/SurveySync/src/SurveySync.Soe/bin/{}/SurveySync.Soe.soe'.format(
    configuration)

user = config.get(credentials, 'username')
password = config.get(credentials, 'password')

data = {'username': user,
        'password': password,
        'client': 'requestip',
        'f': 'json'}

r = requests.post(token_url, data=data)
data = {'f': 'json'}

print 'token aquired'

files = {'itemFile': open(file_name, 'rb'),
         'f': 'json'}

data['token'] = r.json()['token']

print 'uploading...'
r = requests.post(upload_url.format(data['token']), files=files)

print r.status_code, r.json()['status']

data['id'] = r.json()['item']['itemID']

print 'updating item:', data['id']
r = requests.post(update_soe_url, params=data)

print r.status_code, r.json()['status']

print 'starting service...'
r = requests.post(
    start_service_url, params={'f': 'json', 'token': data['token']})

print r.status_code, r.json()['status']
