node default {
	notify { 'YOLO': }
}

node 'taas-es' {
	include windowsfunctions
	include nirvanaservice
	include eventstore
	include javaserverjre
#	include elasticsearch
	include neo4j

#	elasticsearch::plugin { 'marvel': }
	package { 'procexp':
		ensure => '15.13',
	}
}
