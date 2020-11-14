cd C:\git\bat_sh_ps\

docker --version

docker pull docker.elastic.co/elasticsearch/elasticsearch:6.4.2

docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:6.4.2